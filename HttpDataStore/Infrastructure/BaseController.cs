
using System.Web.Http;
using HttpDataStore.StorageEngine;
namespace HttpDataStore.Infrastructure
{
    public class BaseController : ApiController
    {
        private readonly IStoreData<object> dataStore = new InMemoryStore();

        protected IStoreData<object> DataStore
        {
            get
            {
                return dataStore;
            }
        }
    }
}