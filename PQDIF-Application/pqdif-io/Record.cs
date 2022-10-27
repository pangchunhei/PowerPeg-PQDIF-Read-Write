using Gemstone.PQDIF.Logical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pqdif_io
{
    public class Record
    {
        public Record(string pqdifName, DateTime createDateTime, IList<ChannelDefinition> channelDefinitions, List<ObservationRecord> obsRecords)
        {
            this.pqdifTitle = pqdifName;
            this.createDateTime = createDateTime;
            this.observationRecords = obsRecords;
            this.channelDefinitions = channelDefinitions;
        }

        private string pqdifTitle;
        private DateTime createDateTime;
        private IList<ChannelDefinition> channelDefinitions;
        private List<ObservationRecord> observationRecords;

        public string getPqdifTitle() { 
            return this.pqdifTitle;
        }

        public DateTime getCreateDateTime() { 
            return this.createDateTime;
        }
        public IList<ChannelDefinition> getChannelDefinitions() {
            return this.channelDefinitions;        
        }
        public List<ObservationRecord> getObservationRecords() { 
            return this.observationRecords;
        }

    }
}
