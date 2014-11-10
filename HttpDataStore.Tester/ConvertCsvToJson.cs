using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HttpDataStore.Tester
{
    internal static class ConvertCsvToJson
    {
        public static void FromFile(string csvPath, string jsonPath)
        {
            FromFile(csvPath, jsonPath, new Dictionary<string, string>(), new Dictionary<string, Func<string, dynamic>>(), null);
        }
        public static void FromFile(string csvPath, string jsonPath, Dictionary<string, string> propertyMapper, Dictionary<string, Func<string, dynamic>> customValueMappers)
        {
            FromFile(csvPath, jsonPath, propertyMapper, customValueMappers, null);
        }
        public static void FromFile(string csvPath, string jsonPath, Dictionary<string, string> propertyMapper, Dictionary<string, Func<string, dynamic>> customValueMappers, Func<Dictionary<string, dynamic>, bool> excludeEntryPredicate)
        {
            string[] lines = File.ReadAllLines(csvPath);
            if (lines.Length == 0)
            {
                return;
            }

            string[] properties = ParseProperties(lines[0], propertyMapper);

            List<Dictionary<string, dynamic>> jsonPayload = new List<Dictionary<string, dynamic>>(lines.Length - 1);

            foreach (string line in lines.Skip(1))
            {
                Dictionary<string, dynamic> entry = new Dictionary<string, dynamic>();

                int propertyIndex = 0;
                foreach (string value in CleanCsvEntry(SplitCsvLine(line)))
                {
                    string property = properties[propertyIndex];
                    entry[property] = customValueMappers.ContainsKey(property) ? customValueMappers[property](value) : ParseValue(value);
                    propertyIndex++;
                }

                if (excludeEntryPredicate != null && excludeEntryPredicate(entry))
                {
                    continue;
                }

                jsonPayload.Add(entry);
            }

            File.WriteAllText(jsonPath, JsonConvert.SerializeObject(jsonPayload, Formatting.Indented));

        }

        public static dynamic ParseValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            bool asBool;
            decimal asNumber;
            DateTime asDate;
            if (bool.TryParse(value, out asBool)) return asBool;
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out asNumber)) return asNumber;
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out asDate)) return asDate;
            return value;
        }

        private static string[] ParseProperties(string csvColumns, Dictionary<string, string> propertyMapper)
        {
            return CleanCsvEntry(SplitCsvLine(csvColumns))
                .Select(column => propertyMapper.ContainsKey(column) ? propertyMapper[column] : column)
                .ToArray();

        }

        private static string[] SplitCsvLine(string line)
        {
            if (line[0] == ',')
            {
                line = ' ' + line;
            }
            Regex splitter = new Regex("(?:^|,)(?=[^\"]|(\")?)\"?((?(1)[^\"]*|[^,\"]*))\"?(?=,|$)");
            var matches = splitter.Matches(line);
            List<string> values = new List<string>();

            for (var i = 0; i < matches.Count; i++ )
            {
                values.Add(matches[i].Groups[2].Value);
            }

            return values.ToArray();
        }

        private static string[] CleanCsvEntry(string[] csvEntries)
        {
            return csvEntries.Select(entry =>
            {
                if (string.IsNullOrWhiteSpace(entry))
                {
                    return null;
                }

                string clean = entry;
                if (clean[0] == '"')
                {
                    clean = clean.Substring(1);
                }
                if (clean[clean.Length - 1] == '"')
                {
                    clean = clean.Substring(0, clean.Length - 1);
                }

                return clean;

            }).ToArray();
        }
    }
}
