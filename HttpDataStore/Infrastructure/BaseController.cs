
using HttpDataStore.StorageEngine;
using System.Web;
using System.Web.Http;
namespace HttpDataStore.Infrastructure
{
    public class BaseController : ApiController
    {
        private readonly StoreRepository storeRepository;

        public BaseController()
        {
            storeRepository = HttpContext.Current.Application["StoreRepository"] as StoreRepository;
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
    }
}