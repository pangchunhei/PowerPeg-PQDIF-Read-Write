using Gemstone.PQDIF.Logical;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pqdif_io
{
    public class PQDIFGateway
    {
        public PQDIFGateway(string filePath)
        {
            this.pqdifFilePath = filePath;
        }

        private string pqdifFilePath;
        private static readonly ILog log = LogHelper.getLogger();

        public async Task<Record> importPQDifFile()
        {
            log.Info($"Import PQDIF file from: {this.pqdifFilePath}");

            //DataSourceRecord dataSource;

            //File io
            await using LogicalParser parser = new LogicalParser(this.pqdifFilePath);

            //Observation record raw data
            List<ObservationRecord> observationRecords;
            await parser.OpenAsync();
            observationRecords = new List<ObservationRecord>();
            while (await parser.HasNextObservationRecordAsync())
            {
                observationRecords.Add(await parser.NextObservationRecordAsync());
            }
            await parser.CloseAsync();

            string pqdifTitle = parser.ContainerRecord.FileName;
            DateTime createDateTime = parser.ContainerRecord.Creation;
            IList<ChannelDefinition> channelDefinitions = parser.DataSourceRecords[0].ChannelDefinitions;

            return new Record(pqdifTitle, createDateTime, channelDefinitions, observationRecords);
        }
    }
}
