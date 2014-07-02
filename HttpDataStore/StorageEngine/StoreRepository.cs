
using System;
using System.Collections.Generic;
namespace HttpDataStore.StorageEngine
{
    public class StoreRepository
    {
        public const string BlobStoreName = "Blobs";
        private readonly Func<string, IStoreData<object>> defaultFactory;
        private readonly Dictionary<string, IStoreData<object>> store = new Dictionary<string, IStoreData<object>>();
        private readonly Dictionary<string, Func<string, IStoreData<object>>> storeFactory = new Dictionary<string, Func<string, IStoreData<object>>>();

        public StoreRepository(Func<string, IStoreData<object>> defaultFactory)
        {
            this.defaultFactory = defaultFactory;
        }

        public IStoreData<object> BlobStore
        {
            get 
            {
                EnsureStore(BlobStoreName);
                return store[BlobStoreName];
            }
        }

        public void Register(string storeName, Func<string, IStoreData<object>> factory)
        {
            ValidateStoreName(storeName);
            if (factory == null)
            {
                throw new ArgumentException("Parameter factory cannot be null");
            }

            this.storeFactory[storeName] = factory;
        }

        private static void ValidateStoreName(string storeName)
        {
            if (string.IsNullOrWhiteSpace(storeName))
            {
                throw new ArgumentException("Parameter storeName cannot be null or empty");
            }
            if (storeName.Trim().Equals(BlobStoreName, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException("The given store name is reserved, please use another identifier");
            }
        }

        public IStoreData<object> On(string storeName)
        {
            ValidateStoreName(storeName);
            EnsureStore(storeName);
            return store[storeName];
        }

        private void EnsureStore(string storeName)
        {
            if (store.ContainsKey(storeName))
            {
                return;
            }

            var factory = storeFactory.ContainsKey(storeName) ? storeFactory[storeName] : defaultFactory;
            store.Add(storeName, factory(storeName));
        }
    }
}