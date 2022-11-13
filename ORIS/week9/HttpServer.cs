using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using ORIS.week9.Attributes;
using System.Collections.Specialized;

namespace ORIS.week9
{
    public class HttpServer : IDisposable
    {
        private readonly HttpListener listener;
        public ServerStatus Status = ServerStatus.Stop;
        private ServerSettings serverSettings;

        public HttpServer()
        {
            serverSettings = ServerSettings.Deserialize();
            listener = new HttpListener();
            listener.Prefixes.Add($"http://localhost:" + serverSettings.Port + "/");
        }

        public void Start()
        {
            if (Status == ServerStatus.Start)
            {
                Console.WriteLine("Сервер уже запущен!");
            }
            else
            {
                Console.WriteLine("Запуск сервера");
                listener.Start();
                Console.WriteLine("Сервер запущен");
                Status = ServerStatus.Start;
            }
            Receive();
        }

        public void Stop()
        {
            if (Status == ServerStatus.Start)
            {
                Console.WriteLine("Остановка сервера...");
                listener.Stop();
                Console.WriteLine("Сервер остановлен");
                Status = ServerStatus.Stop;
            }
            else
                Console.WriteLine("Сервер уже остановлен");
        }

        private async void Receive()
        {
            while (listener.IsListening)
            {
                var context = await listener.GetContextAsync();

                if (MethodHandler(context)) return;

                StaticFiles(context.Request, context.Response);
            }
            //listener.BeginGetContext(new AsyncCallback(ListenerCallback), listener);
        }

        private void StaticFiles(HttpListenerRequest request, HttpListenerResponse response)
        {
            byte[] buffer;

            if (Directory.Exists(serverSettings.Path))
            {
                buffer = getFile(request.RawUrl.Replace("%20", " "));

                //Задаю расширения для файлов
                Files.GetExtension(ref response, "." + request.RawUrl);

                if (buffer == null)
                {
                    response.Headers.Set("Content-Type", "text/plain");

                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    string err = "404 - not found";

                    buffer = Encoding.UTF8.GetBytes(err);
                }
            }
            else
            {
                string err = $"Directory '{serverSettings.Path}' not found";

                buffer = Encoding.UTF8.GetBytes(err);
            }

            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            // закрываем поток
            output.Close();
        }

        private byte[] getFile(string rawUrl)
        {
            byte[] buffer = null;
            var filePath = serverSettings.Path + rawUrl;

            if (Directory.Exists(filePath))
            {
                // Каталог
                filePath = filePath + "/index.html";
                if (File.Exists(filePath))
                {
                    buffer = File.ReadAllBytes(filePath);
                }
            }
            else if (File.Exists(filePath))
            {
                // Файл
                buffer = File.ReadAllBytes(filePath);
            }

            return buffer;
        }

        public void Dispose()
        {
            listener.Stop();
        }

        private bool MethodHandler(HttpListenerContext context)
        {
            //объект запроса
            HttpListenerRequest request = context.Request;

            //объект ответа
            HttpListenerResponse response = context.Response;

            Console.WriteLine("URL: " + request.Url);
            Console.WriteLine("Http Method: " + request.HttpMethod);
            Console.WriteLine("Content Type: " + request.ContentType);
            Console.WriteLine("Content Length: " + request.ContentLength64);
            Console.WriteLine("Content (Encoded): " + request.ContentEncoding);
            Console.WriteLine("URL Segments: " + string.Join(", ", request.Url.Segments));

            if (request.Url.Segments.Length < 2) return false;

            string controllerName = request.Url.Segments[1].Replace("/", "");

            string[] strParams = request.Url
                .Segments
                .Skip(2)
                .Select(s => s.Replace("/", ""))
                .ToArray();

            var assembly = Assembly.GetExecutingAssembly();

            var controller = assembly.GetTypes().Where(t => Attribute.IsDefined(t, typeof(HttpController)))
                .FirstOrDefault(c => string.Equals(c.Name, controllerName, StringComparison.CurrentCultureIgnoreCase));

            if (controller == null) return false;

            var test = typeof(HttpController).Name;

            string methodURI = strParams[0];

            var method = controller
                .GetMethods()
                .Where(t => t.GetCustomAttributes(true)
                    .Any(attr => attr.GetType().Name == $"Http{request.HttpMethod}"))
                .FirstOrDefault(x => request.HttpMethod switch
            {
                "GET" => x.GetCustomAttribute<HttpGET>()?.MethodURI == methodURI,
                "POST" => x.GetCustomAttribute<HttpPOST>()?.MethodURI == methodURI
            });

            NameValueCollection par = new NameValueCollection();

            if (request.HttpMethod == "GET")
            {
                par = request.QueryString;
            }
            else if (request.HttpMethod == "POST")
            {
                if (request.HasEntityBody)
                {
                    string s;
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        s = reader.ReadToEnd();
                    }

                    string[] parameters = s.Split("&");
                    foreach (string parameter in parameters)
                    {
                        string[] parSplit = parameter.Split('=');
                        par.Add(parSplit[0], parSplit[1]);
                    }
                }
                else
                {
                    Console.WriteLine("No client data was sent with the request.");
                }
            }

            strParams = new string[par.Count];
            for (int i = 0; i < strParams.Length; i++)
            {
                strParams[i] = par.Get(i);
            }

            object[] queryParams = method.GetParameters()
                                .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
                                .ToArray();

            Console.Write("Attributes: ");
            foreach (string? key in par.AllKeys)
                Console.Write(key + " : " + par[key] + ", ");
            Console.WriteLine();

            Console.WriteLine("MethodName: " + method.Name);
            Console.WriteLine("StrParams: " + string.Join(", ", strParams));
            Console.WriteLine("MethodURI: " + methodURI);
            Console.WriteLine("QueryParams: " + queryParams);

            var ret = method.Invoke(Activator.CreateInstance(controller), queryParams);

            response.ContentType = "Application/json";

            byte[] buffer = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(ret));
            response.ContentLength64 = buffer.Length;

            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            output.Close();

            return true;
        }
    }
}