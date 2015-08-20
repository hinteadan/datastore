using HttpDataStore.Model;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpDataStore.Tester
{
    internal static class Azure
    {
        internal static void Run()
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureStorage"].ConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference("dummy");
            table.CreateIfNotExists();

            //InsertNewPerson(table);

            var query = new TableQuery<HTableEntity<Person>>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Person"));
            var persons = table.ExecuteQuery(query).ToArray().Select(r => r.GetInnerEntity());
        }

        private static void InsertNewPerson(CloudTable table)
        {
            var entity = new Entity<Person>(new Person
            {
                Name = "Hintea Dan Alexandru",
                Email = "hinteadan@yahoo.co.uk",
                Address = "Cornesti 37",
                Age = 30,
                Birthday = new DateTime(1985, 1, 28)
            }, new Dictionary<string, object> {
                { "Name", "Hintea Dan Alexandru" },
                { "Email", "hinteadan@yahoo.co.uk" },
                { "Age", 30 },
                { "Birthday", new DateTime(1985, 1, 28) }
            });
            entity.LastModifiedOn = DateTime.Now;

            var tableEntity = HTableEntity<Person>.FromEntity(entity, "Person");

            var insertOperation = TableOperation.Insert(tableEntity);

            table.Execute(insertOperation);
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int Age { get; set; }
        public DateTime Birthday { get; set; }
    }

    public class HTableEntity<T> : TableEntity
    {
        protected Entity<T> entity;

        public HTableEntity() { }

        public static HTableEntity<T> FromEntity(Entity<T> entity, string database = "Default")
        {
            return new HTableEntity<T>
            {
                RowKey = entity.Id.ToString(),
                PartitionKey = database,
                CheckTag = entity.CheckTag,
                LastModifiedOn = entity.LastModifiedOn
            }
            .SetEntity(entity);
        }

        public Guid CheckTag { get; set; }
        public DateTime LastModifiedOn { get; set; }

        private HTableEntity<T> SetEntity(Entity<T> entity)
        {
            this.entity = entity;
            return this;
        }

        public Entity<T> GetInnerEntity()
        {
            return this.entity;
        }

        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var properties = base.WriteEntity(operationContext);
            foreach (var key in entity.Meta.Keys)
            {
                properties.Add(key, EntityProperty.CreateEntityPropertyFromObject(entity.Meta[key]));
            }
            properties.Add("Payload", new EntityProperty(JsonConvert.SerializeObject(entity.Data)));
            return properties;
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            var nonMetaKeys = new string[] { "LastModifiedOn", "CheckTag", "Payload" };

            var meta = properties.Where(p => !nonMetaKeys.Contains(p.Key)).ToDictionary(x => x.Key, x => x.Value.PropertyAsObject);

            this.entity = new Entity<T>(
                Guid.Parse(this.RowKey),
                JsonConvert.DeserializeObject<T>(properties["Payload"].StringValue),
                meta,
                properties["CheckTag"].GuidValue.Value
                )
            { LastModifiedOn = properties["LastModifiedOn"].DateTime.Value };

            base.ReadEntity(properties, operationContext);
        }
    }
}
