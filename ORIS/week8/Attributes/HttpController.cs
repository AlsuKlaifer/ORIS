﻿namespace ORIS.week8.Attributes
{
    public class HttpController : Attribute
    {
        public string ControllerName;

        public HttpController(string controllerName)
        {
            ControllerName = controllerName;
        }
    }
}