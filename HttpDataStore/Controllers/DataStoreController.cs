using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HttpDataStore.Controllers
{
    public class DataStoreController : ApiController
    {
        public IEnumerable<object> Get()
        {
            return new string[] { "SomeData1" };
        }

        public object Get(string id)
        {
            return new { Id = id, Data = "SomeData" };
        }

        public HttpResponseMessage Put(object data)
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        public HttpResponseMessage Delete(string id)
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
