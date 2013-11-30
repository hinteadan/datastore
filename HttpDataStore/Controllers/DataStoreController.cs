using System;
using System.Net;
using System.Net.Http;
using HttpDataStore.Infrastructure;
using HttpDataStore.Model;

namespace HttpDataStore.Controllers
{
    public class DataStoreController : BaseController
    {
        public Entity<object>[] Get()
        {
            return new QueryDataStore(DataStore).Query(Request.RequestUri.ParseQueryString());
        }

        public Entity<object> Get(Guid id)
        {
            return DataStore.Load(id);
        }

        public HttpResponseMessage Put(Entity<object> data)
        {
            DataStore.Save(data);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        public HttpResponseMessage Delete(Guid id)
        {
            DataStore.Delete(id);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
