using Gemstone.PQDIF.Logical;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pqdif_io
{
    public class ProcessFile
    {
        public ProcessFile(string pqdOgFileName, Record record)
        {
            this.pqdifRecord = record;
            this.csvGateway = new CSVGateway(pqdOgFileName, record.getPqdifTitle());
        }

        private CSVGateway csvGateway;
        private Record pqdifRecord;
        private static readonly ILog log = LogHelper.getLogger();

        private ChannelDefinition matchChannelDefinitation(ChannelInstance targetChannel)
        {
            uint idx = targetChannel.ChannelDefinitionIndex;

            IList<ChannelDefinition> channelDefinitions = this.pqdifRecord.getChannelDefinitions();

            return channelDefinitions[checked((int)idx)];
        }

        public void processObservationRecords()
        {
            string pqdifTitle = "File Name:," + this.pqdifRecord.getPqdifTitle() + ",Creation Date:" + this.pqdifRecord.getCreateDateTime();

            log.Info($"Process PQDIF file: {pqdifTitle}");

            csvGateway.saveLineToCSV(pqdifTitle);

            List<ObservationRecord> observationRecords = this.pqdifRecord.getObservationRecords();

            foreach (var observationRecord in observationRecords)
            {
                translateRecord(observationRecord);
            }
        }

        private void translateRecord(ObservationRecord targetObservation)
        {
            DateTime startTime = targetObservation.StartTime;

            foreach (ChannelInstance channelinstance in targetObservation.ChannelInstances)
            {
                ChannelDefinition channeldefinition = matchChannelDefinitation(channelinstance);

                string title = "Channel Defination: " + channeldefinition.ChannelName + " Group Name: " + channelinstance.ChannelGroupID;

                for (int i = 0; i < channelinstance.SeriesInstances.Count; i++)
                {
                    Guid tagIdx = channeldefinition.SeriesDefinitions[i].ValueTypeID;
                    string fieldName = SeriesValueType.ToString(tagIdx);

                    IList<object> data = channelinstance.SeriesInstances[i].OriginalValues;

                    //Transklate Field
                    if (fieldName.Equals("Time"))
                    {
                        IList<Object> adjusted = new List<Object>();
                        foreach (Double dt in data)
                        {
                            DateTime startTimeClone = startTime.AddSeconds(dt);
                            adjusted.Add(startTimeClone);
                        }

                        data = adjusted;
                    }

                    string raw = string.Join(",", data);
                    string exportLine = title + "," + fieldName + "," + raw;

                    log.Info($"Process PQDIF file: {exportLine}");

                    csvGateway.saveLineToCSV(exportLine);
                }
            }
        }
    }
}
