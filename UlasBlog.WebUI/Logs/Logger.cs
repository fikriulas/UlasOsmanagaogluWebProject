
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UlasBlog.WebUI.Logs
{
    public class Logger : ILogger
    {
        IHostingEnvironment _hostingEnvironment;
        public Logger(IHostingEnvironment hostingEnvironment) => _hostingEnvironment = hostingEnvironment;
        public IDisposable BeginScope<TState>(TState state) => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public async void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            /*using (StreamWriter streamWriter = new StreamWriter($"{_hostingEnvironment.ContentRootPath}/log.txt", true))
            {
                streamWriter.WriteLine($"Log Level : {logLevel.ToString()} | Event ID : {eventId.Id} | Event Name : {eventId.Name} | Formatter : {formatter(state, exception)}");
                               
            }
            */
            //var filePath = $"{_hostingEnvironment.ContentRootPath}/log.txt";
            if (logLevel.ToString() != "Information")
            {
                using (var fileStream = File.Open($"{_hostingEnvironment.ContentRootPath}/log.txt", FileMode.Append, FileAccess.Write, FileShare.Write))
                {
                    DateTime dateTime = DateTime.Now;
                    var logMessage = $"-- Date: {dateTime.Date} |Log Level : {logLevel.ToString()} | Event ID : {eventId.Id} | Exception : {(exception == null ? "Null" : exception.Message)} | Formatter : {formatter(state, exception)} {Environment.NewLine}";
                    byte[] logMessageByteArray = Encoding.UTF8.GetBytes(logMessage);
                    await fileStream.WriteAsync(logMessageByteArray);
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }

        }
    }
}
