using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpDataStore.Tester
{
    internal static class ConvertCsvToJson
    {
        public static void FromFile(string csvPath, string jsonPath)
        {
            FromFile(csvPath, jsonPath, new Dictionary<string, string>(), new Dictionary<string, Func<string, dynamic>>());
        }
        public static void FromFile(string csvPath, string jsonPath, Dictionary<string, string> propertyMapper, Dictionary<string, Func<string, dynamic>> customValueMappers)
        {
            string[] lines = File.ReadAllLines(csvPath);
            if (lines.Length == 0)
            {
                return;
            }

            string[] properties = ParseProperties(lines[0], propertyMapper);

        }

        private static string[] ParseProperties(string csvColumns, Dictionary<string, string> propertyMapper)
        {
            return CleanColumnNames(csvColumns.Split(','))
                .Select(column => propertyMapper.ContainsKey(column) ? propertyMapper[column] : column)
                .ToArray();

        }

        private static string[] CleanColumnNames(string[] csvColumns)
        {
            return csvColumns.Select(column => {

                string clean = column;
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
