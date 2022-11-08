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
            //Test waveform
            //translateRecord(observationRecords[120]);
        }

        private void translateRecord(ObservationRecord targetObservation)
        {
            DateTime startTime = targetObservation.StartTime;

            foreach (ChannelInstance channelinstance in targetObservation.ChannelInstances)
            {
                string exportObsChannelRecord = "";
                
                ChannelDefinition channeldefinition = matchChannelDefinitation(channelinstance);

                for (int i = 0; i < channelinstance.SeriesInstances.Count; i++)
                {
                    Guid tagIdx = channeldefinition.SeriesDefinitions[i].ValueTypeID;
                    string fieldName = SeriesValueType.ToString(tagIdx);

                    IList<object> data = channelinstance.SeriesInstances[i].OriginalValues;

                    //Adjustment on Field
                    if (fieldName.Equals("Time"))
                    {
                        IList<object> adjusted = new List<object>();

                        //For wavefom super small datetime second
                        double lastFullSecAdd = -1;
                        int lastFullSec = -1;

                        for(int j = 0; j < data.Count; j++)
                        {
                            double dt = (double)data[j];
                            DateTime startTimeClone;
                            startTimeClone = startTime.AddSeconds(dt);

                            if (channeldefinition.ChannelName.Contains("Waveform"))
                            {
                                if(startTimeClone.Second != lastFullSec)
                                {
                                    //Start of first second
                                    lastFullSec = startTimeClone.Second;
                                    lastFullSecAdd = dt;
                                }

                                double exactMS = startTimeClone.Second + dt - lastFullSecAdd;
                                string adjStrTime = startTimeClone.ToString("yyyy-MM-ddTHH:mm:") + exactMS;
                                adjusted.Add(adjStrTime);
                            }
                            else
                            {
                                adjusted.Add(startTimeClone.ToString("yyyy-MM-ddTHH:mm:ss.ffffff"));
                            }
                        }

                        data = adjusted;
                    }

                    //Skip null data row"
                    if (data.Count == 0)
                    {
                        exportObsChannelRecord = "";
                        break;
                    }

                    string raw = string.Join(",", data);
                    exportObsChannelRecord += "Channel Defination," + channeldefinition.ChannelName + ",Group Name," + channelinstance.ChannelGroupID + "," + fieldName + "," + raw + Environment.NewLine;

                    //log.Debug($"Process PQDIF file: {exportObsChannelRecord}");
                }

                csvGateway.saveSectionToCSV(exportObsChannelRecord);
            }
        }
    }
}
