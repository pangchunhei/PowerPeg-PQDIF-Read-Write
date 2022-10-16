using System;
using Gemstone.PQDIF.Logical;
using Gemstone.PQDIF;

namespace pqdif_file_io
{
    public class tools
    {
        public tools()
        {

        }

        private DataSourceRecord dataSource;
        private IList<ChannelDefinition> channelDefinitions;
        private List<ObservationRecord> observationRecords;

        public async Task importPQDifFile(string filePath)
        {
            //File io
            await using LogicalParser parser = new LogicalParser(filePath);
            await parser.OpenAsync();

            //Observation record raw data
            observationRecords = new List<ObservationRecord>();
            while (await parser.HasNextObservationRecordAsync())
            {
                observationRecords.Add(await parser.NextObservationRecordAsync());
            }

            //Structure mapping
            dataSource = parser.DataSourceRecords[0];
            channelDefinitions = dataSource.ChannelDefinitions;

            /*
            foreach (ObservationRecord observationRecord in observationRecords)
            {
                Console.Write(observationRecord.StartTime);

                // Get the first channel instance
                ChannelInstance firstChannelInstance = observationRecord.ChannelInstances[0];

                List<string> valueName = new List<string>();
                List<string> valueId = new List<string>();

                foreach (ChannelDefinition def in observationRecord.DataSource.ChannelDefinitions)
                {
                    valueName.Add(def.ChannelName);
                    valueId.Add(def.QuantityTypeID.ToString());
                }

                string csv2 = string.Join(",", valueName);
                string csv3 = string.Join(",", valueId);
                Console.WriteLine(csv2);
                Console.WriteLine(csv3);

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
            */

            printRecord(observationRecords[36]);

        }

        public ChannelDefinition matchChannelDefinitation(ChannelInstance targetChannel)
        {
            uint idx = targetChannel.ChannelDefinitionIndex;

            return channelDefinitions[checked((int)idx)];
        }

        public void printRecord(ObservationRecord targetObservation)
        {
            foreach(ChannelInstance channelinstance in targetObservation.ChannelInstances)
            {
                ChannelDefinition channeldefinition = matchChannelDefinitation(channelinstance);

                int groupIdx = channelinstance.ChannelGroupID;

                //Console.WriteLine("Channel Defination: " + channeldefinition.ChannelName + " Group Name: " + groupIdx);
                string title = "Channel Defination: " + channeldefinition.ChannelName + " Group Name: " + groupIdx;

                for (int i = 0; i<channelinstance.SeriesInstances.Count; i++)
                {

                    Guid tagIdx = channeldefinition.SeriesDefinitions[i].ValueTypeID;
                    string fieldName = SeriesValueType.ToString(tagIdx);
                    IList<object> data = channelinstance.SeriesInstances[i].OriginalValues;
                    string raw = string.Join(",", data);

                    Console.WriteLine(title + ", " + fieldName + ", " + raw);
                }
            }
        }
    }
}
