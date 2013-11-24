using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace HttpDataStore
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { controller = "DataStore", id = RouteParameter.Optional }
            );

            config.Filters.Add(new CacheFilter());

            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(
                config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml")
                );
        }

        class CacheFilter : System.Web.Http.Filters.ActionFilterAttribute
        {
            public override void OnActionExecuted(System.Web.Http.Filters.HttpActionExecutedContext context)
            {
                base.OnActionExecuted(context);
                if (context.Request.Method == System.Net.Http.HttpMethod.Get)
                {
                    context.Response.Content.Headers.Expires = DateTimeOffset.Now.AddHours(24);
                    context.Response.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
                    {
                        MaxAge = TimeSpan.FromHours(24),
                        MustRevalidate = true,
                        Private = true
                    };
                }

            }
        }
    }
}
