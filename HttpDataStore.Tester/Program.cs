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
            var response = (new Store("Tester")).Query();

            Console.WriteLine("Response content:\r\n{0}", response);
            Console.WriteLine("\r\nDone. Press key to exit.");
            Console.ReadKey();
        }
    }
}
