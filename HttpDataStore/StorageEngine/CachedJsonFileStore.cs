using HttpDataStore.Model;
using Metrics;
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

        private readonly Metrics.Timer timerForSave = Metric.Context("CachedJsonFileStore").Timer("Save Entity", Unit.Requests);
        private readonly Metrics.Timer timerForLoad = Metric.Context("CachedJsonFileStore").Timer("Load Entity", Unit.Requests);
        private readonly Metrics.Timer timerForQuery = Metric.Context("CachedJsonFileStore").Timer("Query for Entities", Unit.Requests);
        private readonly Metrics.Timer timerForDelete = Metric.Context("CachedJsonFileStore").Timer("Delete Entity", Unit.Requests);

        private readonly Metrics.Timer timerForAlterValidation = Metric.Context("CachedJsonFileStore").Timer("Alter Validation", Unit.Requests);

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
            using (timerForSave.NewContext(data.Id.ToString()))
            {
                ValidateAlterOperationFor(data);
                cache.Set(data.Id.ToString(), data, new CacheItemPolicy());
                return base.SaveWithoutAlterValidation(data);
            }
        }

        public override Entity<object> Load(Guid id)
        {
            using (timerForLoad.NewContext(id.ToString()))
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
        }

        public override IEnumerable<Entity<object>> Query(Func<Dictionary<string, object>, bool> metaDataPredicate)
        {
            using (timerForQuery.NewContext())
            {
                var ids = base.metaStore.Where(v => metaDataPredicate(v.Value)).Select(v => v.Key);
                return ids.Select(id => Load(id)).Where(d => d != null).ToArray();
            }
        }

        public override void Delete(Guid id)
        {
            using (timerForDelete.NewContext(id.ToString()))
            {
                cache.Remove(id.ToString());
                base.Delete(id);
            }
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
    }
}