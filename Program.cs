using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraceRemoteAccessingServer
{
    class Program
    {
        static void Main(string[] args)
        {
            LogConfiguration.ConfigureLog();
            Log.Information("Execution Start....");
            EventLogInfo eventLogInfo = new EventLogInfo();
            eventLogInfo.GetEventLogs();
            Log.Information("Execution End....");
            Console.ReadKey();
        }
    }
}
