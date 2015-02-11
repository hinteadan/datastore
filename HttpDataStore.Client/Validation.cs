using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HttpDataStore.Model;
using Newtonsoft.Json;

namespace HttpDataStore.Client
{
    public class Validation<T>
    {
        private readonly string validationUrl;

        public Validation(string storeUrl)
        {
            this.validationUrl = string.Format("{0}{1}", storeUrl, "validate/");
        }
        public Validation()
            : this("http://localhost/HttpDataStore/")
        { }

        public string QueueForValidation(Entity<T> entity, string storeName, string clientId)
        {
            string serializedObject = JsonConvert.SerializeObject(entity);
            HttpWebRequest request = WebRequest.CreateHttp(string.Format("{0}?storeName={1}&clientId={2}", validationUrl, storeName, clientId));
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
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                return JsonConvert.DeserializeObject<string>(reader.ReadToEnd());
            }
        }

        public Entity<T> Validate(string validationToken, string clientId)
        {
            HttpWebRequest request = WebRequest.CreateHttp(string.Format("{0}?clientId={1}&validationToken={2}", validationUrl, clientId, validationToken));
            request.Method = "POST";
            request.AllowWriteStreamBuffering = false;
            request.ContentType = "application/json";
            request.Accept = "Accept=application/json";
            request.SendChunked = false;
            request.ContentLength = 0;
            var response = request.GetResponse() as HttpWebResponse;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                return JsonConvert.DeserializeObject<Entity<T>>(reader.ReadToEnd());
            }
        }
    }
}
