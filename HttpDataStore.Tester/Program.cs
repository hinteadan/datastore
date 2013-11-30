using System;
using System.Collections.Generic;
using HttpDataStore.Client;
using HttpDataStore.Model;

namespace HttpDataStore.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            var store = new Store<object>("Tester");

            var entities = new Entity<object>[] { 
                new Entity<object>(null, new Dictionary<string, object>() { 
                    { "Name", "Dummy1" },
                    { "Time", DateTime.Today }
                }),
                new Entity<object>(null, new Dictionary<string, object>() { 
                    { "Name", "Dummy2" },
                    { "Time", DateTime.Today.AddDays(2) }
                })
            };

            store.Save(entities[0]);
            store.Save(entities[1]);

            var queryResult = store.QueryMeta(
                new QueryParameter { Name = "Time", Value = DateTime.Today }
                );

            queryResult = store.QueryMeta();

            Console.WriteLine("Query response content:\r\n{0}", queryResult);
            store.Delete(entities[0].Id);
            store.Delete(entities[1].Id);

            Console.WriteLine("\r\nDone. Press key to exit.");
            Console.ReadKey();
        }
    }
}
