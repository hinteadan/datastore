using System.Linq;
using System.Web.Http;
using HttpDataStore.StorageEngine;

namespace HttpDataStore
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
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
                name: "DefaultApi",
                routeTemplate: "{storeName}/{id}",
                defaults: new { controller = "DataStore", storeName = "Default", id = RouteParameter.Optional }
            );

            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(
                config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml")
                );
        }
    }
}
