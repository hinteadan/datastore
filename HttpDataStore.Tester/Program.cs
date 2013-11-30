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
                    { "Name", "Dummy1" }
                }),
                new Entity<object>(null, new Dictionary<string, object>() { 
                    { "Name", "Dummy2" }
                })
            };

            store.Save(entities[0]);
            store.Save(entities[1]);

            var queryResult = store.Query(
                new QueryParameter { Name = "Name", Value = "Dummy1" }
                );

            Console.WriteLine("Query response content:\r\n{0}", queryResult);
            store.Delete(entities[0].Id);
            store.Delete(entities[1].Id);

            Console.WriteLine("\r\nDone. Press key to exit.");
            Console.ReadKey();
        }
    }
}
