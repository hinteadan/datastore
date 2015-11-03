using HttpDataStore.Model;
using HttpDataStore.Store;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpDataStore.AzureStore
{
    public class AzureStore<T> : IStoreData<T>
    {
        private readonly string tableName;
        private readonly CloudStorageAccount storageAccount;
        private readonly CloudTableClient tableClient;

        public AzureStore(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The given Azure table name is not valid", "name");
            }
            tableName = name;
            storageAccount = CloudStorageAccount.Parse(ReadConnectionStringFromConfig());
            tableClient = storageAccount.CreateCloudTableClient();
        }

        public void Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Entity<T> Load(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Entity<T>> Query(Func<Dictionary<string, object>, bool> metaDataPredicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<Guid, Dictionary<string, object>>> QueryMeta(Func<Dictionary<string, object>, bool> metaDataPredicate)
        {
            throw new NotImplementedException();
        }

        public Entity<T> Save(Entity<T> data)
        {
            throw new NotImplementedException();
        }

        private string ReadConnectionStringFromConfig()
        {
            throw new NotImplementedException();
        }
    }
}
