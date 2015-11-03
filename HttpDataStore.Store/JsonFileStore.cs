using HttpDataStore.Model;
using Metrics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;

namespace HttpDataStore.Store
{
    public class JsonFileStore : IStoreData<object>
    {
        private readonly DirectoryInfo storeDirectory;
        private readonly string metaStoreFilePath;
        protected readonly Dictionary<Guid, Dictionary<string, object>> metaStore;

        private readonly Metrics.Timer timerForSave = Metric.Context("JsonFileStore").Timer("Save Entity", Unit.Requests);
        private readonly Metrics.Timer timerForLoad = Metric.Context("JsonFileStore").Timer("Load Entity", Unit.Requests);
        private readonly Metrics.Timer timerForQuery = Metric.Context("JsonFileStore").Timer("Query for Entities", Unit.Requests);
        private readonly Metrics.Timer timerForDelete = Metric.Context("JsonFileStore").Timer("Delete Entity", Unit.Requests);

        private readonly Metrics.Timer timerForMetaUpdate = Metric.Context("JsonFileStore").Timer("Meta-data Update", Unit.Requests);
        private readonly Metrics.Timer timerForAlterValidation = Metric.Context("JsonFileStore").Timer("Alter Validation", Unit.Requests);

        public JsonFileStore() : this(null) { }
        public JsonFileStore(string storeName)
        {
            string basePath = ConfigurationManager.AppSettings["JsonFileStore.StorePath"] ??
                string.Format("{0}JsonFileStore", AppDomain.CurrentDomain.BaseDirectory);
            string path = string.IsNullOrWhiteSpace(storeName) ? basePath : string.Format("{0}\\{1}", basePath, storeName);

            this.storeDirectory = new DirectoryInfo(path);
            if (!storeDirectory.Exists)
            {
                storeDirectory.Create();
            }
            metaStoreFilePath = GenerateMetaStoreFilePath();
            metaStore = InitializeMetaStore();
        }

        public JsonFileStore(string storeBasePath, string storeName)
        {
            if (string.IsNullOrWhiteSpace(storeBasePath))
            {
                throw new ArgumentException("Parameter cannot be null or empty", "storeDirectoryPath");
            }
            string path = string.IsNullOrWhiteSpace(storeName) ? storeBasePath : string.Format("{0}\\{1}", storeBasePath, storeName);
            this.storeDirectory = new DirectoryInfo(path);
            if (!storeDirectory.Exists)
            {
                storeDirectory.Create();
            }
            metaStoreFilePath = GenerateMetaStoreFilePath();
            metaStore = InitializeMetaStore();
        }

        public virtual Entity<object> Save(Entity<object> data)
        {
            using (timerForSave.NewContext(data.Id.ToString()))
            {
                ValidateAlterOperationFor(data);
                return SaveWithoutAlterValidation(data);
            }
        }

        protected Entity<object> SaveWithoutAlterValidation(Entity<object> data)
        {
            File.WriteAllText(
                GenerateDataFilePath(data.Id),
                JsonConvert.SerializeObject(data)
                );
            metaStore[data.Id] = data.Meta;
            UpdateMetaFile();
            return data;
        }

        public virtual Entity<object> Load(Guid id)
        {
            using (timerForLoad.NewContext(id.ToString()))
            {
                string filePath = GenerateDataFilePath(id);
                if (!File.Exists(filePath))
                {
                    return null;
                }
                return JsonConvert.DeserializeObject<Entity<object>>(File.ReadAllText(filePath));
            }
        }

        public virtual IEnumerable<Entity<object>> Query(Func<Dictionary<string, object>, bool> metaDataPredicate)
        {
            using (timerForQuery.NewContext())
            {
                var ids = metaStore.Where(v => metaDataPredicate(v.Value)).Select(v => v.Key);
                return ids.Select(id => Load(id)).Where(d => d != null).ToArray();
            }
        }

        public virtual void Delete(Guid id)
        {
            using (timerForDelete.NewContext(id.ToString()))
            {
                File.Delete(GenerateDataFilePath(id));
                metaStore.Remove(id);
                UpdateMetaFile();
            }
        }

        public virtual IEnumerable<KeyValuePair<Guid, Dictionary<string, object>>> QueryMeta(Func<Dictionary<string, object>, bool> metaDataPredicate)
        {
            return metaStore.Where(e => metaDataPredicate(e.Value)).ToArray();
        }

        private string GenerateDataFilePath(Guid id)
        {
            return string.Format("{0}\\{1}", storeDirectory.FullName, id);
        }

        private string GenerateMetaStoreFilePath()
        {
            return string.Format("{0}\\{1}", storeDirectory.FullName, "Metadata");
        }

        private void UpdateMetaFile()
        {
            using (timerForMetaUpdate.NewContext())
            {
                RunWithRetry(UpdateAndPersistMetadata, 30, TimeSpan.FromSeconds(1));
            }
        }

        private void UpdateAndPersistMetadata()
        {
            File.WriteAllText(
                metaStoreFilePath,
                JsonConvert.SerializeObject(metaStore)
                );
        }

        private Dictionary<Guid, Dictionary<string, object>> InitializeMetaStore()
        {
            if (!File.Exists(metaStoreFilePath))
            {
                return new Dictionary<Guid, Dictionary<string, object>>();
            }

            return JsonConvert.DeserializeObject<Dictionary<Guid, Dictionary<string, object>>>(
                File.ReadAllText(metaStoreFilePath)
                );
        }

        private void ValidateAlterOperationFor(Entity<object> entity)
        {
            using (timerForAlterValidation.NewContext(entity.Id.ToString()))
            {
                var exsitingEntity = Load(entity.Id);
                if (exsitingEntity == null)
                {
                    entity.PinAlterPoint();
                    return;
                }
                exsitingEntity.ValidateAlterOperation(entity);
                exsitingEntity.PinAlterPoint();
                entity.PinAlterPoint();
            }
        }

        private void RunWithRetry(Action process, int maxRetries, TimeSpan retryInterval)
        {
            int count = 0;
            while (count < maxRetries)
            {
                try
                {
                    count++;
                    process();

                }
                catch (Exception)
                {
                    Thread.Sleep(retryInterval);
                    continue;
                }
            }
        }
    }
}