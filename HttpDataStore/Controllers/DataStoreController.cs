using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using HttpDataStore.Infrastructure;
using HttpDataStore.Model;

namespace HttpDataStore.Controllers
{
    public class DataStoreController : BaseController
    {
        public HttpResponseMessage Get()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            if (queryString.AllKeys.Where(k => k != "chainWith" && !string.IsNullOrWhiteSpace(k)).Count() == 0)
            {
                return new HttpResponseMessage(HttpStatusCode.RequestEntityTooLarge);
            }
            return Request.CreateResponse(HttpStatusCode.OK, new QueryDataStore(DataStore).Query(queryString));
        }

        public HttpResponseMessage Get(Guid id)
        {
            var entity = DataStore.Load(id);
            if(entity == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            return Request.CreateResponse(HttpStatusCode.OK, entity);
        }

        public HttpResponseMessage Put(Entity<object> data)
        {
            DataStore.Save(data);
            return Request.CreateResponse(HttpStatusCode.OK, data.Id);
        }

        public HttpResponseMessage Delete(Guid id)
        {
            DataStore.Delete(id);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
