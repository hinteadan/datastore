using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using HttpDataStore.Model;
using Newtonsoft.Json;

namespace HttpDataStore.StorageEngine
{
    public class JsonFileStore : IStoreData<object>
    {
        private readonly DirectoryInfo storeDirectory;
        private readonly string metaStoreFilePath;
        private readonly Dictionary<Guid, Dictionary<string, object>> metaStore = new Dictionary<Guid, Dictionary<string, object>>();

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
        }

        public Entity<object> Save(Entity<object> data)
        {
            File.WriteAllText(
                GenerateDataFilePath(data.Id),
                JsonConvert.SerializeObject(data)
                );
            metaStore[data.Id] = data.Meta;
            UpdateMetaFile();
            return data;
        }

        public Entity<object> Load(Guid id)
        {
            return JsonConvert.DeserializeObject<Entity<object>>(
                File.ReadAllText(GenerateDataFilePath(id))
                );
        }

        public IEnumerable<Entity<object>> Query(Func<Dictionary<string, object>, bool> metaDataPredicate)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid id)
        {
            File.Delete(GenerateDataFilePath(id));
            metaStore.Remove(id);
            UpdateMetaFile();
        }

        internal string GenerateDataFilePath(Guid id)
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
    }
}