using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpDataStore.Client
{
    public class Store
    {
        private readonly string storeName;
        private readonly string storeUrl = "http://localhost/HttpDataStore/";

        public Store(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Store name cannot be empty", "name");
            }

            this.storeName = name;
        }

        public string Query()
        {
            HttpWebRequest request = WebRequest.CreateHttp(storeUrl);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Accept = "Accept=application/json";

            var response = request.GetResponse() as HttpWebResponse;

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

        public void Save(object entity)
        {
            string serializedObject = Newtonsoft.Json.JsonConvert.SerializeObject(entity);
            HttpWebRequest request = WebRequest.CreateHttp(storeUrl);
            request.Method = "PUT";
            request.AllowWriteStreamBuffering = false;
            request.ContentType = "application/json";
            request.Accept = "Accept=application/json";
            request.SendChunked = false;
            request.ContentLength = serializedObject.Length;
            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(serializedObject);
            }

            var response = request.GetResponse() as HttpWebResponse;
        }
    }
}
