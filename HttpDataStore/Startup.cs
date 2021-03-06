﻿using System.Net.Http.Formatting;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using Owin.Metrics;
using System.Linq;
using Microsoft.Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Metrics;
using System;

[assembly: OwinStartup(typeof(HttpDataStore.Startup))]
namespace HttpDataStore
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "MetaApi",
                routeTemplate: "meta/{storeName}",
                defaults: new { controller = "DataStoreMeta", storeName = "Default", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "BlobApi",
                routeTemplate: "blob/{id}",
                defaults: new { controller = "Blob", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "ValidationApi",
                routeTemplate: "validate",
                defaults: new { controller = "Validation", storeName = "Default", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{storeName}/{id}",
                defaults: new { controller = "DataStore", storeName = "Default", id = RouteParameter.Optional }
            );

            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(
                config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml")
                );

            Metric.Config
                .WithAllCounters()
                .WithReporting(r => r.WithConsoleReport(TimeSpan.FromSeconds(30)))
                .WithOwin(middleware => app.Use(middleware), cfg => cfg
                    .WithRequestMetricsConfig(c => c.WithAllOwinMetrics())
                    .WithMetricsEndpoint()
                );

            app
                .UseCors(CorsOptions.AllowAll)
                .MapSignalR("/realtime", new HubConfiguration())
                .UseWebApi(config);
        }
    }
}
