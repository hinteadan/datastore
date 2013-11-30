using System;
using System.Collections.Generic;
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

        public Entity<T>[] Query(params QueryParameter[] queryParams)
        {
            return Query(ChainOperation.And, queryParams);
        }
        public Entity<T>[] Query(ChainOperation chain, params QueryParameter[] queryParams)
        {
            HttpWebRequest request = WebRequest.CreateHttp(GenerateQueryUrl(chain, queryParams));
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Accept = "Accept=application/json";

            var response = request.GetResponse() as HttpWebResponse;

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                return JsonConvert.DeserializeObject<Entity<T>[]>(reader.ReadToEnd());
            }
        }

        public KeyValuePair<Guid, Dictionary<string, object>>[] QueryMeta(params QueryParameter[] queryParams)
        {
            return QueryMeta(ChainOperation.And, queryParams);
        }
        public KeyValuePair<Guid, Dictionary<string, object>>[] QueryMeta(ChainOperation chain, params QueryParameter[] queryParams)
        {
            HttpWebRequest request = WebRequest.CreateHttp(GenerateQueryMetaUrl(chain, queryParams));
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Accept = "Accept=application/json";

            var response = request.GetResponse() as HttpWebResponse;

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                return JsonConvert.DeserializeObject<KeyValuePair<Guid, Dictionary<string, object>>[]>(reader.ReadToEnd());
            }
        }

        private string GenerateQueryUrl(ChainOperation chain, IEnumerable<QueryParameter> queryParams)
        {
            return string.Format("{0}?chainWith={1}&{2}", storeUrl, chain, string.Join("&", queryParams));
        }

        private string GenerateQueryMetaUrl(ChainOperation chain, IEnumerable<QueryParameter> queryParams)
        {
            return string.Format("{0}meta?chainWith={1}&{2}", storeUrl, chain, string.Join("&", queryParams));
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
