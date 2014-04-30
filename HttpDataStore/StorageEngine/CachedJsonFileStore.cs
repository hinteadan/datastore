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
            ValidateAlterOperationFor(data);
            cache.Set(data.Id.ToString(), data, new CacheItemPolicy());
            return base.SaveWithoutAlterValidation(data);
        }

        public override Entity<object> Load(Guid id)
        {
            if (cache.Contains(id.ToString()))
            {
                return cache[id.ToString()] as Entity<object>;
            }
            var entity = base.Load(id);
            if (entity != null)
            {
                cache.Add(id.ToString(), entity, new CacheItemPolicy());
            }
            return entity;
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

        private void ValidateAlterOperationFor(Entity<object> entity)
        {
            var exsitingEntity = Load(entity.Id);
            if (exsitingEntity == null)
            {
                return;
            }
            exsitingEntity.ValidateAlterOperation(entity);
        }
    }
}