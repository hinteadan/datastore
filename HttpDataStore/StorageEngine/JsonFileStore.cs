using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using HttpDataStore.Model;
using Newtonsoft.Json;

namespace HttpDataStore.StorageEngine
{
    public class JsonFileStore : IStoreData<object>
    {
        private readonly DirectoryInfo storeDirectory;
        private readonly string metaStoreFilePath;
        protected readonly Dictionary<Guid, Dictionary<string, object>> metaStore;

        public JsonFileStore()
        {
            string path = ConfigurationManager.AppSettings["JsonFileStore.StorePath"];
            if (string.IsNullOrWhiteSpace(path))
            {
                path = string.Format("{0}JsonFileStore", AppDomain.CurrentDomain.BaseDirectory);
            }

            this.storeDirectory = new DirectoryInfo(path);
            if (!storeDirectory.Exists)
            {
                storeDirectory.Create();
            }
            metaStoreFilePath = GenerateMetaStoreFilePath();
            metaStore = InitializeMetaStore();
        }

        public JsonFileStore(string storeDirectoryPath)
        {
            if (string.IsNullOrWhiteSpace(storeDirectoryPath))
            {
                throw new ArgumentException("Paramter cannot be null or empty", "storeDirectoryPath");
            }

            this.storeDirectory = new DirectoryInfo(storeDirectoryPath);
            if (!storeDirectory.Exists)
            {
                storeDirectory.Create();
            }
            metaStoreFilePath = GenerateMetaStoreFilePath();
            metaStore = InitializeMetaStore();
        }

        public virtual Entity<object> Save(Entity<object> data)
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
            return JsonConvert.DeserializeObject<Entity<object>>(
                File.ReadAllText(GenerateDataFilePath(id))
                );
        }

        public virtual IEnumerable<Entity<object>> Query(Func<Dictionary<string, object>, bool> metaDataPredicate)
        {
            var ids = metaStore.Where(v => metaDataPredicate(v.Value)).Select(v => v.Key);
            return ids.Select(id => Load(id)).Where(d => d != null).ToArray();
        }

        public virtual void Delete(Guid id)
        {
            File.Delete(GenerateDataFilePath(id));
            metaStore.Remove(id);
            UpdateMetaFile();
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
    }
}