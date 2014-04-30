using HttpDataStore.Infrastructure;
using HttpDataStore.Model;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace HttpDataStore.Controllers
{
    public class DataStoreController : BaseController
    {
        public HttpResponseMessage Get(string storeName)
        {
            var queryString = Request.RequestUri.ParseQueryString();
            if (queryString.AllKeys.Where(k => k != "chainWith" && !string.IsNullOrWhiteSpace(k)).Count() == 0)
            {
                return new HttpResponseMessage(HttpStatusCode.RequestEntityTooLarge);
            }
            return Request.CreateResponse(HttpStatusCode.OK, new QueryDataStore(Store.On(storeName)).Query(queryString));
        }

        public HttpResponseMessage Get(Guid id, string storeName)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, Store.On(storeName).Load(id));
            }
            catch (System.IO.FileNotFoundException)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }

        public HttpResponseMessage Put(Entity<object> data, string storeName)
        {
            Store.On(storeName).Save(data);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        public HttpResponseMessage Delete(Guid id, string storeName)
        {
            Store.On(storeName).Delete(id);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
