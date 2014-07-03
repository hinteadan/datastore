using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using HttpDataStore.Infrastructure;
using HttpDataStore.Model;
using HttpDataStore.StorageEngine;

namespace HttpDataStore.Controllers
{
    public class BlobController : BaseController
    {
        private readonly string blobsFolderPath;

        public BlobController() 
            : base() 
        {
            this.blobsFolderPath = ConfigurationManager.AppSettings["Blobs.Folder"] ?? 
                string.Format(@"{0}{1}", AppDomain.CurrentDomain.BaseDirectory, StoreRepository.BlobStoreName);
        }

        public async Task<HttpResponseMessage> PostFile()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            EnsureBlobsFolder(blobsFolderPath);

            var provider = new MultipartFormDataStreamProvider(blobsFolderPath);

            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);

                var entities = GenerateEntitiesAndRenameBlobs(provider).ToArray();

                foreach (var entity in entities)
                {
                    BlobStore.Save(entity);
                }

                return Request.CreateResponse(HttpStatusCode.OK, entities);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        private void EnsureBlobsFolder(string blobsFolderPath)
        {
            if (Directory.Exists(blobsFolderPath))
            {
                return;
            }
            Directory.CreateDirectory(blobsFolderPath);
        }

        private IEnumerable<Entity<object>> GenerateEntitiesAndRenameBlobs(MultipartFormDataStreamProvider provider) 
        {
            var commonMeta = provider.FormData.AllKeys.ToDictionary(k => k, k => (object)string.Join(",", provider.FormData.GetValues(k)));

            foreach (var file in provider.FileData)
            {
                string originalFileName = file.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                string originalFileExtension = originalFileName.Substring(originalFileName.LastIndexOf('.') + 1);
                FileInfo fileInfo = new FileInfo(file.LocalFileName);
                var entity = new Entity<object>(null, new Dictionary<string, object>(commonMeta));
                entity.Meta.Add("HDS-CreationTime", fileInfo.CreationTime);
                entity.Meta.Add("HDS-Extension", originalFileExtension);
                entity.Meta.Add("HDS-Length", fileInfo.Length);
                entity.Meta.Add("HDS-Name", originalFileName);
                fileInfo.MoveTo(string.Format("{0}\\{1}", blobsFolderPath, entity.Id.ToString()));
                yield return entity;
            }
        }

        public KeyValuePair<Guid, Dictionary<string, object>>[] Get()
        {
            return new QueryDataStore(BlobStore).QueryMeta(Request.RequestUri.ParseQueryString());
        }

        public HttpResponseMessage Get(Guid id)
        {
            try
            {
                var path = string.Format("{0}\\{1}", this.blobsFolderPath, id);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(new FileStream(path, FileMode.Open));
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                return response;
            }
            catch (System.IO.FileNotFoundException)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }

        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}