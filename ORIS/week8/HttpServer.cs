using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using ORIS.week8.Attributes;

namespace ORIS.week8
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

        //private void ListenerCallback(IAsyncResult result)
        //{
        //    if (listener.IsListening)
        //    {
        //        var context = listener.EndGetContext(result);
        //        var request = context.Request;

        //        // получаем объект ответа
        //        var response = context.Response;

        //        var rawUrl = request.RawUrl;

        //        byte[] buffer;

        //        if (Directory.Exists(serverSettings.Path))
        //        {
        //            buffer = Files.getFile(rawUrl.Replace("%20", " "));

        //            //Задаю расширения для файлов
        //            Files.GetExtension(ref response, "." + rawUrl);

        //            if (buffer == null)
        //            {
        //                response.Headers.Set("Content-Type", "text/plain");

        //                response.StatusCode = (int)HttpStatusCode.NotFound;
        //                string err = "404 - not found";
        //                buffer = Encoding.UTF8.GetBytes(err);
        //            }
        //        }
        //        else
        //        {
        //            string err = $"Directory '{serverSettings.Path}' doesn't found";
        //            buffer = Encoding.UTF8.GetBytes(err);
        //        }

        //        // получаем поток ответа и пишем в него ответ
        //        Stream output = response.OutputStream;
        //        output.Write(buffer, 0, buffer.Length);

        //        // закрываем поток
        //        output.Close();

        //        Receive();
        //    }
        //}

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

            if (context.Request.Url.Segments.Length < 3) return false;

            string controllerName = context.Request.Url.Segments[1].Replace("/", "");

            string[] strParams = context.Request.Url
                                    .Segments
                                    .Skip(2)
                                    .Select(s => s.Replace("/", ""))
                                    .ToArray();

            var assembly = Assembly.GetExecutingAssembly();

            var controller = assembly.GetTypes().Where(t => Attribute.IsDefined(t, typeof(HttpController))).FirstOrDefault(c => c.Name.ToLower() == controllerName.ToLower());

            if (controller == null) return false;

            var test = typeof(HttpController).Name;
            var method = controller.GetMethods().Where(t => t.GetCustomAttributes(true).Any(attr => attr.GetType().Name == $"Http{context.Request.HttpMethod}")).FirstOrDefault();

            object[] queryParams = method.GetParameters()
                .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
                                    .ToArray();


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