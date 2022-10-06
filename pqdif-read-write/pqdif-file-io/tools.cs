using System;
using Gemstone.PQDIF.Logical;
using Gemstone.PQDIF.Physical;

namespace pqdif_file_io
{
    public class tools
    {
        public static async Task defaultRead(string filePath)
        {
            ContainerRecord containerRecord;
            List<ObservationRecord> observationRecords = new List<ObservationRecord>();

            await using LogicalParser parser = new LogicalParser(filePath);
            await parser.OpenAsync();
            containerRecord = parser.ContainerRecord;

            while (await parser.HasNextObservationRecordAsync())
            {
                observationRecords.Add(await parser.NextObservationRecordAsync());
            }

            foreach(var v in observationRecords)
            {
                Console.WriteLine(v.PhysicalRecord.Body);
            }
        }
    }
}
