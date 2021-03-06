﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace ASPNETWebAPI_Token.Custom
{
    public class CustomControllerSelector : DefaultHttpControllerSelector
    {
        private HttpConfiguration _config;

        public CustomControllerSelector(HttpConfiguration configuration) : base(configuration)
        {
            _config = configuration;
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            // Get all avaiable web api controllers
            var controllers = GetControllerMapping();
            // Get the controller name and parameter values from request URI
            var routeData = request.GetRouteData();
            // Get the controller name from route data
            var controllerName = routeData.Values["controller"].ToString();

            string versionNumber = "1";
            //var versionQueryString = HttpUtility.ParseQueryString(request.RequestUri.Query);
            //if (versionQueryString["v"] != null)
            //    versionNumber = versionQueryString["v"];

            // Get version number from custom header
            //string customHeader = "X-StudentService-Version";
            //if (request.Headers.Contains(customHeader))
            //{
            //    versionNumber = request.Headers.GetValues(customHeader).FirstOrDefault();
            //}

            // Get version number from accept header
            var acceptHeader = request.Headers.Accept.Where(a => a.Parameters.Count(p => p.Name.ToLower() == "version") > 0);
            if (acceptHeader.Any())
                versionNumber = acceptHeader.First().Parameters.First(p => p.Name.ToLower() == "version").Value;

            controllerName = versionNumber == "1" ? controllerName : controllerName + "V2";

            HttpControllerDescriptor controllerDescriptor;
            if (controllers.TryGetValue(controllerName, out controllerDescriptor))
                return controllerDescriptor;
            return null;
        }
    }
}