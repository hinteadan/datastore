
using System;
using System.Collections.Generic;
namespace HttpDataStore.StorageEngine
{
    public class StoreRepository
    {
        private readonly Func<string, IStoreData<object>> defaultFactory;
        private readonly Dictionary<string, IStoreData<object>> store = new Dictionary<string, IStoreData<object>>();
        private readonly Dictionary<string, Func<string, IStoreData<object>>> storeFactory = new Dictionary<string, Func<string, IStoreData<object>>>();

        public StoreRepository(Func<string, IStoreData<object>> defaultFactory)
        {
            this.defaultFactory = defaultFactory;
        }

        public void Register(string storeName, Func<string, IStoreData<object>> factory)
        {
            if (string.IsNullOrWhiteSpace(storeName))
            {
                throw new ArgumentException("Parameter storeName cannot be null or empty");
            }
            if (factory == null)
            {
                throw new ArgumentException("Parameter factory cannot be null");
            }

            this.storeFactory[storeName] = factory;
        }

        public IStoreData<object> On(string storeName)
        {
            if (string.IsNullOrWhiteSpace(storeName))
            {
                throw new ArgumentException("Parameter storeName cannot be null or empty");
            }

            if (!store.ContainsKey(storeName))
            {
                var factory = storeFactory.ContainsKey(storeName) ? storeFactory[storeName] : defaultFactory;
                store.Add(storeName, factory(storeName));
            }

            return store[storeName];
        }
    }
}