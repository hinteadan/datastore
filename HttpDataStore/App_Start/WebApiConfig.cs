﻿using System.Linq;
using System.Web.Http;

namespace HttpDataStore
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "MetaApi",
                routeTemplate: "meta/",
                defaults: new { controller = "DataStoreMeta", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{id}",
                defaults: new { controller = "DataStore", id = RouteParameter.Optional }
            );

            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(
                config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml")
                );
        }
    }
}
