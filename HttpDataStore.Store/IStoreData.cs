
using System;
using System.Collections.Generic;
using HttpDataStore.Model;

namespace HttpDataStore.Store
{
    public interface IStoreData<T>
    {
        Entity<T> Save(Entity<T> data);
        Entity<T> Load(Guid id);
        IEnumerable<Entity<T>> Query(Func<Dictionary<string, object>, bool> metaDataPredicate);
        IEnumerable<KeyValuePair<Guid, Dictionary<string, object>>> QueryMeta(Func<Dictionary<string, object>, bool> metaDataPredicate);
        void Delete(Guid id);
    }
}
