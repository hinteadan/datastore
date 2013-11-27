using System;
using System.IO;
using System.Net;
using HttpDataStore.Model;
using Newtonsoft.Json;

namespace HttpDataStore.Client
{
    public class Store<T>
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

        public Entity<T>[] Query()
        {
            HttpWebRequest request = WebRequest.CreateHttp(storeUrl);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Accept = "Accept=application/json";

            var response = request.GetResponse() as HttpWebResponse;

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                return JsonConvert.DeserializeObject<Entity<T>[]>(reader.ReadToEnd());
            }
        }

        public Entity<T> Load(Guid id)
        {
            HttpWebRequest request = WebRequest.CreateHttp(string.Format("{0}/{1}", storeUrl, id));
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Accept = "Accept=application/json";

            var response = request.GetResponse() as HttpWebResponse;

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                return JsonConvert.DeserializeObject<Entity<T>>(reader.ReadToEnd());
            }
        }

        public void Save(Entity<T> entity)
        {
            string serializedObject = JsonConvert.SerializeObject(entity);
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

        public void Delete(Guid id)
        {
            HttpWebRequest request = WebRequest.CreateHttp(string.Format("{0}/{1}", storeUrl, id));
            request.Method = "DELETE";
            request.ContentType = "application/json";
            request.Accept = "Accept=application/json";
            request.GetResponse();
        }
    }
}
