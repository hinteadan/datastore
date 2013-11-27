using System;
using HttpDataStore.Client;
using HttpDataStore.Model;

namespace HttpDataStore.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            var store = new Store<object>("Tester");
            Console.WriteLine("Query response content:\r\n{0}", store.Query());
            Console.WriteLine("Load response content:\r\n{0}", store.Load(Guid.Empty));

            store.Save(new Entity<object>());
            store.Delete(Guid.Empty);

            Console.WriteLine("\r\nDone. Press key to exit.");
            Console.ReadKey();
        }
    }
}
