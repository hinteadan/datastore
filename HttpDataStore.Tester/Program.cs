using HttpDataStore.Client;
using HttpDataStore.Model;
using System;
using System.Collections.Generic;

namespace HttpDataStore.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            var store = new Store<object>("Test2");

            var entities = new Entity<object>[] { 
                new Entity<object>(null, new Dictionary<string, object>() { 
                    { "Name", "Dummy1" },
                    { "Time", DateTime.Today }
                })
            };

            store.Save(entities[0]);

            var queryResult = store.QueryMeta(
                new QueryParameter { Name = "Time", Value = DateTime.Today }
                );

            queryResult = store.QueryMeta();

            Console.WriteLine("Query response content:\r\n{0}", queryResult);
            store.Delete(entities[0].Id);

            Console.WriteLine("\r\nDone. Press key to exit.");
            Console.ReadKey();
        }
    }
}
