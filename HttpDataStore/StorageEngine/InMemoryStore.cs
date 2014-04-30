
using System;
using System.Collections.Generic;
using System.Linq;
using HttpDataStore.Model;
namespace HttpDataStore.StorageEngine
{
    internal class InMemoryStore : IStoreData<object>
    {
        private readonly Dictionary<Guid, Entity<object>> dataStore = new Dictionary<Guid, Entity<object>>();
        private readonly Dictionary<Guid, Dictionary<string, object>> metaStore = new Dictionary<Guid, Dictionary<string, object>>();

        public Entity<object> Save(Entity<object> data)
        {
            ValidateAlterOperationFor(data);
            dataStore[data.Id] = data;
            metaStore[data.Id] = data.Meta;
            return data;
        }

        public Entity<object> Load(Guid id)
        {
            return dataStore[id];
        }

        public IEnumerable<Entity<object>> Query(Func<Dictionary<string, object>, bool> metaDataPredicate)
        {
            var ids = metaStore.Where(v => metaDataPredicate(v.Value)).Select(v => v.Key);
            return dataStore.Where(e => ids.Contains(e.Key)).Select(e => e.Value);
        }

        public void Delete(Guid id)
        {
            metaStore.Remove(id);
            dataStore.Remove(id);
        }

        public IEnumerable<KeyValuePair<Guid, Dictionary<string, object>>> QueryMeta(Func<Dictionary<string, object>, bool> metaDataPredicate)
        {
            return metaStore.Where(e => metaDataPredicate(e.Value)).ToArray();
        }

        private void ValidateAlterOperationFor(Entity<object> entity)
        {
            var exsitingEntity = Load(entity.Id);
            if(exsitingEntity == null)
            {
                return;
            }
            exsitingEntity.ValidateAlterOperation(entity);
        }
    }
}