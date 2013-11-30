using System;
using System.Collections.Generic;
using System.Net.Http;
using HttpDataStore.Infrastructure;

namespace HttpDataStore.Controllers
{
    public class DataStoreMetaController : BaseController
    {
        public KeyValuePair<Guid, Dictionary<string, object>>[] Get()
        {
            return new QueryDataStore(DataStore).QueryMeta(Request.RequestUri.ParseQueryString());
        }
    }
}
