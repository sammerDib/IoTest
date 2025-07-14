using CsvHelper;

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using CsvHelper.Configuration;

namespace UnitySC.Beckhoff.Emulator
{
    public static class CsvFileHelper
    {
        private const string CsvDelimiter = ";";

        public static void WriteRecord<T>(T item, string path, bool append = false)
        {
            using (var streamWriter = new StreamWriter(path, append, Encoding.UTF8))
            {
                if (append)
                {
                    streamWriter.WriteLine();
                }

                using (var writer = new CsvWriter(streamWriter, new CsvConfiguration (CultureInfo.InvariantCulture) {Delimiter = CsvDelimiter}))
                {
                    writer.WriteRecord(item);
                }
            }
        }

        public static void WriteRecords<T>(IEnumerable<T> collection, string path, bool append = false)
        {
            using (var streamWriter = new StreamWriter(path, false, Encoding.UTF8))
            {
                if (append)
                {
                    streamWriter.WriteLine();
                }

                using (var writer = new CsvWriter(streamWriter, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = CsvDelimiter }))
                {
                    writer.WriteRecords(collection);
                }
            }
        }

        public static List<T> ReadRecords<T>(string path)
        {
            List<T> records;
            using (var stream = File.OpenRead(path))
            using (var reader = new StreamReader(stream, Encoding.UTF8, true))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = CsvDelimiter }))
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
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = CsvDelimiter }))
            {
                item = csv.GetRecord<T>();
            }
            return item;
        }
    }
}
