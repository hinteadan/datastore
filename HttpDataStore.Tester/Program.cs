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
            //new BlobStore("http://localhost/HDataStore/").Save(@"C:\WORK\ev\Docs\arch\iview-main.png");

            ConvertCsvToJson.FromFile(@"C:\Users\dan.hintea\Downloads\Mobile app contest - Places.csv", @"C:\Users\dan.hintea\Downloads\Mobile app contest - Places.json");

            Console.WriteLine("\r\nDone. Press key to exit.");
            Console.ReadKey();
        }
    }
}
