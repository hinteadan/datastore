using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HttpDataStore.Client;

namespace HttpDataStore.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            var store = new Store("Tester");
            Console.WriteLine("Query response content:\r\n{0}", store.Query());
            Console.WriteLine("Load response content:\r\n{0}", store.Load("39557ca6-693c-4fb6-91d6-89a77d0a3efa"));

            store.Save(new { Id = 1, Whatever = "Some text" });
            store.Delete("d71b9a3f-9569-4d30-a066-838072669e1e");

            Console.WriteLine("\r\nDone. Press key to exit.");
            Console.ReadKey();
        }
    }
}
