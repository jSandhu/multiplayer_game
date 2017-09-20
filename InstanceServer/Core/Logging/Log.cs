using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using log4net.Config;
using System;
using System.IO;

namespace InstanceServer.Core.Logging
{
    /// <summary>
    /// Utility class used for logging server messages.
    /// </summary>
    public static class Log
    {
        private const string START =    "[start]";
        private const string END =      "[ end ]";
        private const string HEADER_SIDE =  " ===================== ";
        private const string FOOTER_SIDE =  " ..................... ";

        private static ILogger _logger;
        private static ILogger logger
        {
            get
            {
                if (_logger == null)
                {
                    throw new Exception("Must call SetupLogger first.");
                }
                return _logger;
            }
        }

        public static void SetupLogger(string applicationRootPath, string applicationName, string binaryPath)
        {
            LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
            _logger = LogManager.GetCurrentClassLogger();
            log4net.GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(applicationRootPath, "log");
            log4net.GlobalContext.Properties["LogFileName"] = applicationName;
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(binaryPath, "log4net.config")));
        }

        public static void InfoHeader(string infoString)
        {
            Info(START + HEADER_SIDE + infoString + HEADER_SIDE);
        }

        public static void InfoFooter(string infoString)
        {
            Info(END + FOOTER_SIDE + infoString + FOOTER_SIDE);
        }

        public static void Info(string infoString)
        {
            logger.InfoFormat(infoString);
        }

        public static void Warning(string warningString)
        {
            logger.WarnFormat(warningString);
        }

        public static void Error(string errorString)
        {
            logger.ErrorFormat(errorString);
        }
    }
}
