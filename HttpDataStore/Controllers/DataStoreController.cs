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
        public IEnumerable<string> Get()
        {
            return new string[] { "SomeData1" };
        }
    }
}
