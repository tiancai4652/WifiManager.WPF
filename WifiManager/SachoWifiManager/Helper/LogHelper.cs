using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SachoWifiManager.Helper
{
    public class LogHelper
    {
        static ILogger logger;

        public static ILogger LogInstance
        {
            get
            {
                if (logger == null)
                {
                    logger = LogManager.GetCurrentClassLogger();
                }
                return logger;
            }
        }

        public static void Debug(string content)
        {
            string msg = content + Environment.NewLine;
            LogInstance.Debug(msg);
        }
    }
}
