using HttpDataStore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace HttpDataStore.StorageEngine
{
    public class CachedJsonFileStore : JsonFileStore
    {
        private readonly Guid cacheRepositoryId = Guid.NewGuid();
        private readonly MemoryCache cache;

        public CachedJsonFileStore()
            : base()
        {
            cache = new MemoryCache(cacheRepositoryId.ToString());
        }

        public CachedJsonFileStore(string storeName)
            : base(storeName)
        {
            cache = new MemoryCache(cacheRepositoryId.ToString());
        }

        public CachedJsonFileStore(string storeBasePath, string storeName)
            : base(storeBasePath, storeName)
        {
            cache = new MemoryCache(cacheRepositoryId.ToString());
        }

        public override Entity<object> Save(Entity<object> data)
        {
            cache.Set(data.Id.ToString(), data, new CacheItemPolicy());
            return base.Save(data);
        }

        public override Entity<object> Load(Guid id)
        {
            return (cache.AddOrGetExisting(id.ToString(), base.Load(id), new CacheItemPolicy()) ?? cache[id.ToString()]) as Entity<object>;
        }

        public override IEnumerable<Entity<object>> Query(Func<Dictionary<string, object>, bool> metaDataPredicate)
        {
            var ids = base.metaStore.Where(v => metaDataPredicate(v.Value)).Select(v => v.Key);
            return ids.Select(id => Load(id)).Where(d => d != null).ToArray();
        }

        public override void Delete(Guid id)
        {
            cache.Remove(id.ToString());
            base.Delete(id);
        }
    }
}