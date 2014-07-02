using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HttpDataStore.Model;

namespace HttpDataStore.Client
{
    public class BlobStore
    {
        private readonly Store<object> store;
        private const string storeName = "blob";
        private readonly string storeUrl;

        public BlobStore(string storeUrl)
        {
            this.storeUrl = storeUrl;
            this.store = new Store<object>(storeName, this.storeUrl);
        }
        public BlobStore()
            : this("http://localhost/HttpDataStore/")
        { }

        public Entity<object>[] Query(params QueryParameter[] queryParams)
        {
            return store.Query(ChainOperation.And, queryParams);
        }
        public Entity<object>[] Query(ChainOperation chain, params QueryParameter[] queryParams)
        {
            return store.Query(chain, queryParams);
        }

        public string Load(Guid id)
        {
            HttpWebRequest request = WebRequest.CreateHttp(string.Format("{0}{1}/{2}", this.storeUrl, storeName, id));
            var response = request.GetResponse();
            using (StreamReader streamReader = new StreamReader(request.GetResponse().GetResponseStream(), Encoding.UTF8))
            {
                return streamReader.ReadToEnd();
            }
        }

        public void Save(string filePath)
        {
            new WebClient().UploadFile(string.Format("{0}{1}", this.storeUrl, storeName), filePath);
        }
    }
}
