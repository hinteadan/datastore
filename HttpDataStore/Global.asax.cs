using System.Web.Http;
using HttpDataStore.StorageEngine;

namespace HttpDataStore
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            this.Context.Application["DataStore"] = new InMemoryStore();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
        }
    }
}