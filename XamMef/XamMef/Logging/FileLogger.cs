using System;
using System.IO;
using System.Collections.Concurrent;
using System.Threading;

namespace MFractor.Logging
{
    public sealed class FileLoggerFactory : ILoggerFactory
    {
        public ILogger Create(string context)
        {
            return new FileLogger(context, null);
        }

        public void Dispose()
        {
        }
    }

    public sealed class FileLogger : LogWriter
    {
        readonly LogFileWriter writer;

        public FileLogger(string context, LogFileWriter writer)
            : base(context)
        {
            this.writer = writer;
        }

        public override void Event(string type, string message, LogLevel logLevel = LogLevel.Event)
        {
            try
            {
                var output = string.Format("{0} [{1} : {2}] - {3}\n", DateTime.Now, Context, type, message);
                writer.WriteToFile(output);
            }
            catch
            {
            }
        }

        public override void Exception(Exception ex)
        {
            try
            {
                var output = string.Format("{0} [{1} : {2}]\n", DateTime.Now, Context, ex);
                writer.WriteToFile(output);
            }
            catch
            {
            }
        }
    }
}

