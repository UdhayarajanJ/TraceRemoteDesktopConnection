using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraceRemoteAccessingServer
{
    public static class LogConfiguration
    {
        public static void ConfigureLog()
        {
            string LogFile = Path.Combine(Environment.CurrentDirectory, "LogInformation", "logs-.txt");

            string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj} {Exception} {NewLine} {NewLine}";

            Log.Logger = new LoggerConfiguration()
                                .MinimumLevel.Information()
                                .WriteTo.Console(outputTemplate: outputTemplate)
                                .WriteTo.File(LogFile, rollingInterval: RollingInterval.Day, outputTemplate: outputTemplate)
                                .CreateLogger();
        }
    }
}
