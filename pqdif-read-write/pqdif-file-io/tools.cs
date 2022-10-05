using System;
using Gemstone.PQDIF.Logical;
//using Gemstone.PQDIF.Logical;
using Gemstone.PQDIF.Physical;

namespace pqdif_file_io
{
    public class tools
    {
        /*public static void read(string fileName)
        {
            LogicalParser parser;

            parser = new LogicalParser(fileName);
            parser.OpenAsync();

            Console.WriteLine(parser.ContainerRecord.PhysicalRecord);
            Console.WriteLine(parser.ContainerRecord.PhysicalRecord.Body.Collection);

            //Task<Record> record;
            //record = parser.GetNextRecordAsync();
            //Console.WriteLine(record.ToString()); 
            //Console.WriteLine();

            Console.ReadLine();
        }*/

        public static void read(string filePath)
        {
            using PhysicalParser parser = new PhysicalParser(filePath);
            parser.OpenAsync();

            while (parser.HasNextRecord())
            {
                Task<Record> record = parser.GetNextRecordAsync();
                Console.WriteLine(record.Result);
                Console.WriteLine();
            }
        }

        public static void defaultLog(string filePath)
        {
            ContainerRecord containerRecord;
            List<ObservationRecord> observationRecords = new List<ObservationRecord>();

            using LogicalParser parser = new LogicalParser(filePath);
            parser.OpenAsync();
            containerRecord = parser.ContainerRecord;
            while (parser.HasNextObservationRecordAsync())
            {
                observationRecords.Add(parser.NextObservationRecordAsync());
            }
        }
}
}
