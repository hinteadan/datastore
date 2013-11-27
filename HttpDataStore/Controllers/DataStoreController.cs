using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HttpDataStore.Model;

namespace HttpDataStore.Controllers
{
    public class DataStoreController : ApiController
    {
        public Entity<object>[] Get()
        {
            return new Entity<object>[] { new Entity<object>() };
        }

        public Entity<object> Get(Guid id)
        {
            return new Entity<object>(id, null as object);
        }

        public HttpResponseMessage Put(Entity<object> data)
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        public HttpResponseMessage Delete(Guid id)
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
