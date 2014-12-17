
using HttpDataStore.StorageEngine;
using System.Web;
using System.Web.Http;
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

        protected RealtimeBroadcaster Broadcast
        {
            get
            {
                return realtimeBroadcaster;
            }
        }
    }
}