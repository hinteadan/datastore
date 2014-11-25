using HttpDataStore.Client;
using HttpDataStore.Model;
using System;
using System.Linq;
using System.Collections.Generic;

namespace HttpDataStore.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            //new BlobStore("http://localhost/HDataStore/").Save(@"C:\WORK\ev\Docs\arch\iview-main.png");

            ConvertHangOutPlacesCsvToJson(@"C:\Users\dan.hintea\Downloads\Mobile app contest - Places.csv", @"C:\Users\dan.hintea\Downloads\Mobile app contest - Places.json");
            ConvertHangOutActivitiesCsvToJson(@"C:\Users\dan.hintea\Downloads\Mobile app contest - Activities.csv", @"C:\Users\dan.hintea\Downloads\Mobile app contest - Activities.json");
            ConvertHangOutCategoriesCsvToJson(@"C:\Users\dan.hintea\Downloads\Mobile app contest - Activity categories.csv", @"C:\Users\dan.hintea\Downloads\Mobile app contest - Activity categories.json");
            Console.WriteLine("\r\nDone. Press key to exit.");
            Console.ReadKey();
        }

        private static void ConvertHangOutCategoriesCsvToJson(string csvFile, string jsonFile)
        {
            ConvertCsvToJson.FromFile(csvFile, jsonFile, new Dictionary<string, string>{
                { "Category", "category" },
                { "Image", "image" }
            }, new Dictionary<string, Func<string, dynamic>> { 
                { "category", value => value == null ? null : value.ToLowerInvariant().Replace("activities", string.Empty).Trim().ToLowerInvariant() }
            }, entry => string.IsNullOrWhiteSpace(entry["category"]) || string.IsNullOrWhiteSpace(entry["image"]) || entry["category"] == "good logo");
        }

        private static void ConvertHangOutPlacesCsvToJson(string csvFile, string jsonFile)
        {
            ConvertCsvToJson.FromFile(csvFile, jsonFile, new Dictionary<string, string>{
                { "Name", "name" },
                { "Address", "address" },
                { "Details", "details" },
                { "Website URL", "websiteUrl" },
                { "GPS Location (lat, long, space separated)", "location" },
                { "Tags (space separated - used to link activity to place)", "tags" },
                { "Logo URL", "logoUrl" }
            }, new Dictionary<string, Func<string, dynamic>> { 
                { "location", value => 
                    {
                        if(string.IsNullOrWhiteSpace(value)){
                            return null;
                        }
                        dynamic[] parts = value.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries).Select(p => ConvertCsvToJson.ParseValue(p.Trim())).ToArray();
                        return new {
                            lat = (decimal)parts[0],
                            lng = (decimal)parts[1],
                        };
                    }
                },
                { "tags", value => 
                    {
                        if(string.IsNullOrWhiteSpace(value)){
                            return new string[0];
                        }
                        return value.Split(' ').Where(v => !string.IsNullOrWhiteSpace(v)).Select(v => v.Trim()).ToArray();
                    }
                },
            }, entry => string.IsNullOrWhiteSpace(entry["name"]));
        }

        private static void ConvertHangOutActivitiesCsvToJson(string csvFile, string jsonFile)
        {
            ConvertCsvToJson.FromFile(csvFile, jsonFile, new Dictionary<string, string>{
                { "Title", "title" },
                { "Category", "categories" },
                { "Keywords (space separated)", "keywords" },
                { "Description", "description" },
                { "Tags (space separated - used to link activity to place)", "tags" },
                { "Picture", "imageUrls" },
                { "Activity Logo", "logoUrl" }
            }, new Dictionary<string, Func<string, dynamic>> { 
                { "categories", value => 
                    {
                        if(string.IsNullOrWhiteSpace(value)){
                            return new string[0];
                        }
                        return value.Split(' ').Where(v => !string.IsNullOrWhiteSpace(v)).Select(v => v.Trim().ToLowerInvariant()).Where(v => v != "activities").Distinct().ToArray();
                    }
                },
                { "keywords", value => 
                    {
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            return new string[0];
                        }
                        return value.Split(' ').Where(v => !string.IsNullOrWhiteSpace(v)).Select(v => v.Trim()).ToArray();
                    }
                },
                { "tags", value => 
                    {
                        if(string.IsNullOrWhiteSpace(value)){
                            return new string[0];
                        }
                        return value.Split(' ').Where(v => !string.IsNullOrWhiteSpace(v)).Select(v => v.Trim()).ToArray();
                    }
                },
                { "imageUrls", value => 
                    {
                        if(string.IsNullOrWhiteSpace(value)){
                            return new string[0];
                        }
                        return value.Split(' ').Where(v => !string.IsNullOrWhiteSpace(v)).Select(v => v.Trim()).ToArray();
                    }
                }
            }, entry => string.IsNullOrWhiteSpace(entry["title"]));
        }
    }
}
