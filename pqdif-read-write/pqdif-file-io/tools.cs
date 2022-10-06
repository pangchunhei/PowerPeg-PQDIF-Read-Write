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

            foreach (ObservationRecord observationRecord in observationRecords)
            {
                Console.Write(observationRecord.StartTime);

                // Get the first channel instance
                ChannelInstance firstChannelInstance = observationRecord.ChannelInstances[0];


                foreach (SeriesInstance seriesInstance in firstChannelInstance.SeriesInstances)
                {
                    // FYI, this expands StorageMethods.Increment and applies
                    // scale/offset and transducer ratio fields to the values,
                    // as opposed to seriesInstance.SeriesValues which returns
                    // the unmodified raw values as a VectorElement
                    IList<object> values = seriesInstance.OriginalValues;
                    string csv1 = string.Join(",", values);
                    Console.WriteLine(csv1);
                }

                Console.WriteLine();
            }

        }
    }
}
