using System;
using Gemstone.PQDIF.Logical;
using Gemstone.PQDIF;

namespace pqdif_io
{
    public class MainFunction
    {
        public MainFunction(string fname = "test_output.csv")
        {
            this.fileName = ".\\" + fname;
            File.WriteAllText(fileName, string.Empty);
        }

        private DataSourceRecord dataSource;
        private IList<ChannelDefinition> channelDefinitions;
        private List<ObservationRecord> observationRecords;

        private string fileName;

        public async Task importPQDifFile(string filePath)
        {
            //File io
            await using LogicalParser parser = new LogicalParser(filePath);

            //Observation record raw data
            await parser.OpenAsync();
            observationRecords = new List<ObservationRecord>();
            while (await parser.HasNextObservationRecordAsync())
            {
                observationRecords.Add(await parser.NextObservationRecordAsync());
            }
            await parser.CloseAsync();

            string pqdifTitle = "File Name:," + parser.ContainerRecord.FileName + ",Creation Date:" + parser.ContainerRecord.Creation + Environment.NewLine;
            saveToCSV(pqdifTitle);

            //Structure mapping
            dataSource = parser.DataSourceRecords[0];
            channelDefinitions = dataSource.ChannelDefinitions;
            
            //Get the value of a observation record from pqdif
            foreach (var observationRecord in observationRecords)
            {
                printRecord(observationRecord);
            }
        }

        public ChannelDefinition matchChannelDefinitation(ChannelInstance targetChannel)
        {
            uint idx = targetChannel.ChannelDefinitionIndex;

            return channelDefinitions[checked((int)idx)];
        }

        public void printRecord(ObservationRecord targetObservation)
        {
            DateTime startTime = targetObservation.StartTime;

            foreach (ChannelInstance channelinstance in targetObservation.ChannelInstances)
            {
                ChannelDefinition channeldefinition = matchChannelDefinitation(channelinstance);

                int groupIdx = channelinstance.ChannelGroupID;

                //Console.WriteLine("Channel Defination: " + channeldefinition.ChannelName + " Group Name: " + groupIdx);
                string title = "Channel Defination: " + channeldefinition.ChannelName + " Group Name: " + groupIdx;

                for (int i = 0; i < channelinstance.SeriesInstances.Count; i++)
                {
                    Guid tagIdx = channeldefinition.SeriesDefinitions[i].ValueTypeID;
                    string fieldName = SeriesValueType.ToString(tagIdx);

                    IList<object> data = channelinstance.SeriesInstances[i].OriginalValues;

                    if (fieldName.Equals("Time"))
                    {
                        IList<Object> adjusted = new List<Object>();
                        foreach(Double dt in data)
                        {
                            DateTime startTimeClone = startTime.AddSeconds(dt);
                            adjusted.Add(startTimeClone);
                        }

                        data = adjusted;
                    }

                    string raw = string.Join(",", data);

                    Console.Write(title + "," + fieldName + "," + raw + Environment.NewLine);

                    saveToCSV(title + "," + fieldName + "," + raw + Environment.NewLine);
                }
            }
        }

        public void saveToCSV(string str)
        {
            File.AppendAllText(this.fileName, str);
        }
    }
}
