namespace ORIS.week9.Attributes
{
    public class HttpPOST : Attribute
    {
        public string MethodURI;
        public HttpPOST(string methodURI)
        {
            MethodURI = methodURI;
        }
    }
}