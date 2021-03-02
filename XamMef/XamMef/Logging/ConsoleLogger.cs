using System;
using System.Diagnostics;

namespace MFractor.Logging
{
    public sealed class ConsoleLoggerFactory : ILoggerFactory
    {
        public ILogger Create(string context)
        {
            return new ConsoleLogger(context);
        }

        public void Dispose()
        {
        }
    }

    public class ConsoleLogger : LogWriter
    {
        public ConsoleLogger(string context)
            : base(context)
        {
        }

        public string Render(string type, string message, LogLevel logLevel = LogLevel.Event)
        {
            return String.Format("{0} [{1} : {2}] {3}", DateTime.Now, Context, type, message);
        }

        public override void Event(string type, string message, LogLevel logLevel = LogLevel.Event)
        {
#if !DEBUG
            if (logLevel == LogLevel.Instrument)
            {
                return;
            }
#endif
            var output = Render(type, message, logLevel);
            //Console.WriteLine(output);
            Console.Out.WriteLine(output);
            System.Diagnostics.Debug.WriteLine(output);
        }

        public override void Exception(Exception ex)
        {
            var output = Render("Exception", ex.ToString(), LogLevel.Error);
            //Console.WriteLine(output);
            Console.Out.WriteLine(output);
            System.Diagnostics.Debug.WriteLine(output);
        }
    }
}

