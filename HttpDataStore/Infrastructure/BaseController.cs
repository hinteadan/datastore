
using HttpDataStore.StorageEngine;
using System.Web;
using System.Web.Http;
using HttpDataStore.Model;
using HttpDataStore.Store;

namespace HttpDataStore.Infrastructure
{
    public class BaseController : ApiController
    {
        private readonly StoreRepository storeRepository;
        private readonly RealtimeBroadcaster realtimeBroadcaster;

        public BaseController()
        {
            storeRepository = HttpContext.Current.Application["StoreRepository"] as StoreRepository;
            realtimeBroadcaster = new RealtimeBroadcaster();
        }

        protected StoreRepository Store
        {
            get
            {
                return storeRepository;
            }
        }

        protected IStoreData<object> BlobStore
        {
            get
            {
                return storeRepository.BlobStore;
            }
        }

        protected IStoreData<object> ValidationStore
        {
            get
            {
                return storeRepository.ValidationStore;
            }
        }

        protected RealtimeBroadcaster Broadcast
        {
            get
            {
                return realtimeBroadcaster;
            }
        }
    }
}