using System;
using System.IO;
using Gemstone.PQDIF.Logical;
using Gemstone.PQDIF;
using System.Configuration;
using log4net;

namespace pqdif_io
{
    public class MainFunction
    {
        private static readonly ILog log = LogHelper.getLogger();

        public static string getDefaultPQDIFFolderPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["BatchImportFolder"];
        }

        public static string getDefaultCSVExportFolder()
        {
            return AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["ExportCSVFolder"];
        }

        public static async Task batchProcessing()
        {
            string pqdifFolder = getDefaultPQDIFFolderPath();

            string[] filePaths = Directory.GetFiles(pqdifFolder, "*.pqd");

            foreach(string filePath in filePaths)
            {
                log.Info($"Prepare process the PQDIF file from: {filePath}");

                PQDIFGateway pqdifGateway = new PQDIFGateway(filePath);

                Record record = await pqdifGateway.importPQDifFile();

                FileProcessor processFile = new FileProcessor(Path.GetFileName(filePath), record);

                processFile.processObservationRecords();
            }
        }
    }
}

//Progress mon: https://bytelanguage.net/2018/07/14/reporting-progress-in-async-method/
