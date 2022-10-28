using log4net;
using System.Configuration;
using System.Data;
using System.Text;

namespace pqdif_io
{
    public class CSVGateway
    {
        public CSVGateway(string pqdOiginalFileName, string fileName)
        {
            string csvFolderPath = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["ExportCSVFolder"];
            log.Debug($"CSV export data folder {csvFolderPath}");

            this.csvFilePath = csvFolderPath + "\\"+ DateTime.Now.ToString("yyyy'-'MM'-'dd'_'HH'-'mm'-'ss") + "_" + pqdOiginalFileName + "_" + fileName + ".csv";

            log.Info($"CSV filepath {this.csvFilePath}");

            File.WriteAllText(this.csvFilePath, string.Empty);
        }

        private static readonly ILog log = LogHelper.getLogger();

        private string csvFilePath;

        public void saveLineToCSV(string lineContent)
        {
            log.Debug($"Save to CSV line {lineContent}");

            File.AppendAllText(this.csvFilePath, lineContent);
        }

    }
}
