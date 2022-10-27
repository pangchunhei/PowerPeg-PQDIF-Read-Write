using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using log4net;

//Keep check the log4net config setting in app config
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace pqdif_io
{
    public static class LogHelper
    {
        /// <summary>
        /// Obtain the file that called the log operation
        /// </summary>
        /// <param name="filename">Caller file path</param>
        /// <returns></returns>
        public static ILog getLogger([CallerFilePath] string filename = "")
        {
            return LogManager.GetLogger(filename);
        }
    }
}
