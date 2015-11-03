
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using HttpDataStore.Model;
using HttpDataStore.Store;

namespace HttpDataStore.StorageEngine
{
    public class StoreRepository
    {
        public const string BlobStoreName = "Blobs";
        public const string ValidationStoreName = "AwaitingValidation";
        private readonly Func<string, IStoreData<object>> defaultFactory;
        private readonly ConcurrentDictionary<string, IStoreData<object>> store = new ConcurrentDictionary<string, IStoreData<object>>();
        private readonly ConcurrentDictionary<string, Func<string, IStoreData<object>>> storeFactory = new ConcurrentDictionary<string, Func<string, IStoreData<object>>>();

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

        public IStoreData<object> ValidationStore
        {
            get
            {
                EnsureStore(ValidationStoreName);
                return store[ValidationStoreName] as IStoreData<object>;
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
            string normalizedName = storeName.Trim();
            if (normalizedName.Equals(BlobStoreName, StringComparison.InvariantCultureIgnoreCase) || normalizedName.Equals(ValidationStoreName, StringComparison.InvariantCultureIgnoreCase))
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
            store.TryAdd(storeName, factory(storeName));
        }
    }
}