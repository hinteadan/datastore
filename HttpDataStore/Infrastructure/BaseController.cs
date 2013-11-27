
using System.Web;
using System.Web.Http;
using HttpDataStore.StorageEngine;
namespace HttpDataStore.Infrastructure
{
    public class BaseController : ApiController
    {
        private readonly IStoreData<object> dataStore;

        public BaseController()
        {
            dataStore = HttpContext.Current.Application["DataStore"] as IStoreData<object>;
        }

        protected IStoreData<object> DataStore
        {
            get
            {
                return dataStore;
            }
        }
    }
}