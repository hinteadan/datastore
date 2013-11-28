using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using HttpDataStore.Model;

namespace HttpDataStore.StorageEngine
{
    public class CachedJsonFileStore : IStoreData<object>
    {
        private readonly Guid cacheRepositoryId = Guid.NewGuid();
        private readonly MemoryCache cache;
        private readonly JsonFileStore fileStore;

        public CachedJsonFileStore()
        {
            this.fileStore = new JsonFileStore();
            cache = new MemoryCache(cacheRepositoryId.ToString());
        }

        public CachedJsonFileStore(string storeDirectoryPath)
        {
            this.fileStore = new JsonFileStore(storeDirectoryPath);
            cache = new MemoryCache(cacheRepositoryId.ToString());
        }

        public Entity<object> Save(Entity<object> data)
        {
            return fileStore.Save(data);
        }

        public Entity<object> Load(Guid id)
        {
            return fileStore.Load(id);
        }

        public IEnumerable<Entity<object>> Query(Func<Dictionary<string, object>, bool> metaDataPredicate)
        {
            return fileStore.Query(metaDataPredicate);
        }

        public void Delete(Guid id)
        {
            fileStore.Delete(id);
        }
    }
}