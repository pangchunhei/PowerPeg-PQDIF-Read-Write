using Gemstone.PQDIF.Logical;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pqdif_io
{
    public class FileProcessor
    {
        public FileProcessor(string customOutputName, Record record)
        {
            this.pqdifRecord = record;
            this.csvGateway = new CSVGateway(customOutputName, record.getPqdifTitle());
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
            string pqdifTitle = "File Title: " + this.pqdifRecord.getPqdifTitle() + " Creation Date: " + this.pqdifRecord.getCreateDateTime();

            log.Info($"Processing PQDIF file: {pqdifTitle}");

            csvGateway.saveLineToCSV(pqdifTitle);

            List<ObservationRecord> observationRecords = this.pqdifRecord.getObservationRecords();
            
            for(int i = 0; i < observationRecords.Count; i++)
            {
                log.Info($"ObservationRecord progress: {i + 1}/{observationRecords.Count}");

                translateRecord(observationRecords[i]);
            }
        }

        private void translateRecord(ObservationRecord targetObservation)
        {
            DateTime startTime = targetObservation.StartTime;

            foreach (ChannelInstance channelinstance in targetObservation.ChannelInstances)
            {
                string exportRecord = "";
                
                ChannelDefinition channeldefinition = matchChannelDefinitation(channelinstance);

                for (int i = 0; i < channelinstance.SeriesInstances.Count; i++)
                {
                    Guid tagIdx = channeldefinition.SeriesDefinitions[i].ValueTypeID;
                    string fieldName = SeriesValueType.ToString(tagIdx);

                    IList<object> data = channelinstance.SeriesInstances[i].OriginalValues;

                    //Adjustment on Field
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

                    //Skip null data row
                    if (data.Count == 0)
                    {
                        exportRecord = "";
                        continue;
                    }

                    string raw = string.Join(",", data);
                    exportRecord += "Channel Defination," + channeldefinition.ChannelName + ",Group Name," + channelinstance.ChannelGroupID + "," + fieldName + "," + raw + Environment.NewLine;

                    //log.Debug($"Process PQDIF file: {exportLine}");
                }

                csvGateway.saveSectionToCSV(exportRecord);
            }
        }
    }
}
