using HttpDataStore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace HttpDataStore.Controllers
{
    public class DataStoreMetaController : BaseController
    {
        public KeyValuePair<Guid, Dictionary<string, object>>[] Get(string storeName)
        {
            return new QueryDataStore(Store.On(storeName)).QueryMeta(Request.RequestUri.ParseQueryString());
        }

        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
