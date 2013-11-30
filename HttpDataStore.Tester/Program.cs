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
            var entity = new Entity<object>(null, new Dictionary<string, object>() { 
                { "Name", "Dummy" }
            });

            store.Save(entity);
            Console.WriteLine("Query response content:\r\n{0}", store.Query(
                new QueryParameter { Name = "Name", Value = "Dummy" },
                new QueryParameter { Name = "Name2", Value = "Dummy" }
                ));
            Console.WriteLine("Load response content:\r\n{0}", store.Load(entity.Id));
            store.Delete(entity.Id);

            Console.WriteLine("\r\nDone. Press key to exit.");
            Console.ReadKey();
        }
    }
}
