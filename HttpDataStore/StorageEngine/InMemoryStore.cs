
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
            dataStore[data.Id] = data;
            return data;
        }

        public Entity<object> Load(Guid id)
        {
            return dataStore[id];
        }

        public IEnumerable<Entity<object>> Query(Func<Dictionary<string, object>, bool> metaDataPredicate)
        {
            var ids = metaStore.Where(v => metaDataPredicate(v.Value)).Select(v => v.Key);
            return dataStore.Where(e => ids.Contains(e.Key)).Select(e => e.Value).ToArray();
        }

        public void Delete(Guid id)
        {
            dataStore.Remove(id);
        }
    }
}