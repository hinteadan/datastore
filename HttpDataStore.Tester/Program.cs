using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpDataStore.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            string storeUrl = @"http://localhost/HttpDataStore/";

            HttpWebRequest request = WebRequest.CreateHttp(storeUrl);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Accept = "Accept=application/json";

            var response = request.GetResponse() as HttpWebResponse;
            string responseContent;
            using(var reader = new StreamReader(response.GetResponseStream()))
            {
                responseContent = reader.ReadToEnd();
            }

            Console.WriteLine("Response content:\r\n{0}", responseContent);
            Console.WriteLine("\r\nDone. Press key to exit.");
            Console.ReadKey();
        }
    }
}
