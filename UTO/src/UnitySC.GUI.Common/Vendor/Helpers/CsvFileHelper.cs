using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using CsvHelper;
using CsvHelper.Configuration;

namespace UnitySC.GUI.Common.Vendor.Helpers
{
    public static class CsvFileHelper
    {
        private static readonly CsvConfiguration CsvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            IncludePrivateMembers = true, Delimiter = ";"
        };

        public static void WriteRecord<T>(T item, string path)
        {
            using (var streamWriter = new StreamWriter(path, false, Encoding.UTF8))
            using (var writer = new CsvWriter(streamWriter, CsvConfiguration))
            {
                writer.WriteHeader<T>();
                writer.NextRecord();
                writer.WriteRecord(item);
            }
        }

        public static void WriteRecords<T>(IEnumerable<T> collection, string path)
        {
            using (var streamWriter = new StreamWriter(path, false, Encoding.UTF8))
            using (var writer = new CsvWriter(streamWriter, CsvConfiguration))
            {
                writer.WriteRecords(collection);
            }
        }

        public static List<T> ReadRecords<T>(string path)
        {
            List<T> records;

            using (var stream = File.OpenRead(path))
            using (var reader = new StreamReader(stream, Encoding.UTF8, true))
            using (var csv = new CsvReader(reader, CsvConfiguration))
            {
                records = csv.GetRecords<T>().ToList();
            }

            return records;
        }

        public static T ReadRecord<T>(string path)
        {
            T item;

            using (var stream = File.OpenRead(path))
            using (var reader = new StreamReader(stream, Encoding.UTF8, true))
            using (var csv = new CsvReader(reader, CsvConfiguration))
            {
                csv.Read();
                item = csv.GetRecord<T>();
            }

            return item;
        }
    }
}
