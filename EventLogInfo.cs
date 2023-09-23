using Dapper;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
//using evtx;
namespace TraceRemoteAccessingServer
{
    public class EventLogInfo
    {
        private string _ipAddress;
        private string _hostName;
        private int _reportInterval;

        public string ipAddress
        {
            get
            {
                return _ipAddress;
            }
            set
            {
                _ipAddress = value;
            }
        }

        public string hostName
        {
            get
            {
                return _hostName;
            }
            set
            {
                _hostName = value;
            }
        }

        public int reportInterval
        {
            get
            {
                return _reportInterval;
            }
            set
            {
                _reportInterval = value;
            }
        }

        public EventLogInfo()
        {
            GetCurrentIP();
            reportInterval = Convert.ToInt32(ConfigurationManager.AppSettings["ReportInterval"]);
        }

        public void GetCurrentIP()
        {
            List<string> _currentIPs = new List<string>();
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    _currentIPs.Add(ip.ToString());
            }
            string currentIPs = string.Join(" - ", _currentIPs);
            (ipAddress, hostName) = (currentIPs, host.HostName);
        }

        public void GetEventLogs()
        { 
            string systemRootPath = Environment.GetEnvironmentVariable("SystemRoot");
            string pathOfLogs = Path.Combine(systemRootPath, @"System32\Winevt\Logs\Microsoft-Windows-TerminalServices-RemoteConnectionManager%4Operational.evtx");
            List<Dictionary<string, string>> logs = new List<Dictionary<string, string>>();
            using (var reader = new EventLogReader(pathOfLogs, PathType.FilePath))
            {
                DateTime dateTimeNow = DateTime.Now;
                DateTime dateTimeOneHour = dateTimeNow.AddDays(-2).AddHours(-reportInterval);
                int totalEvent = 0;
                EventRecord record;
                while ((record = reader.ReadEvent()) != null)
                {
                    using (record)
                    {
                        string test = record.FormatDescription();
                        string[] data = record.FormatDescription().Split('\n');

                        if (record.Id == 1149 && dateTimeOneHour <= record.TimeCreated && record.TimeCreated <= dateTimeNow)
                        {
                            SaveLogInformation(record).Wait();
                            totalEvent += 1;
                        }
                    }
                }
                Log.Information($"Successfully save last {reportInterval} hrs Data : Total saved event : {totalEvent}");
            }
        }
        private async Task SaveLogInformation(EventRecord eventRecord)
        {
            try
            {
                string fullData = eventRecord.FormatDescription();

                string[] splitAllLogData = fullData.Split('\n');

                if (splitAllLogData.Length == 0)
                    return;

                string description = splitAllLogData[0].ToString().Trim();

                string userIpAddress = splitAllLogData[4].ToString().Split(':')[1].ToString().Trim();
                string userHostName = splitAllLogData[3].ToString().Split(':')[1].ToString().Trim();
                string rdpUserName = splitAllLogData[2].ToString().Split(':')[1].ToString().Trim();

                string serverIpAddress = ipAddress;
                string serverHostName = hostName;

                DateTime accessedDateTime = Convert.ToDateTime(eventRecord.TimeCreated);
                string levelName = eventRecord.LevelDisplayName;

                string rawXMLData = eventRecord.ToXml();

                using (DbConnection dbConnection = DapperContext.CreateConnection())
                {
                    var objParam = new
                    {
                        UserIp = userIpAddress,
                        UserHostName = userHostName,
                        RDPUserName = rdpUserName,
                        ServerIp = serverIpAddress,
                        ServerHostName = serverHostName,
                        AccessTime = accessedDateTime,
                        LevelOfEvent = levelName,
                        RawXMLData = rawXMLData,
                        Description = description,
                        IsAbnormal = IsAbnormal(accessedDateTime)
                    };
                    await dbConnection.ExecuteAsync("usp_AccessingAuditLogs", objParam, commandType: System.Data.CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        private bool IsAbnormal(DateTime dateTime)
        {
            int currentHour = dateTime.Hour;
            if (currentHour > 20 || currentHour < 8)
                return true;
            return false;
        }
    }
}
