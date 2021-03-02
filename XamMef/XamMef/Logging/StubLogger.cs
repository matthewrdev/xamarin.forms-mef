using System;

namespace MFractor.Logging
{
    public class StubLogger : ILogger
    {
        public StubLogger(string context)
        {
            Context = context;
        }

        public string Context { get; }

        public void Debug(string message)
        {
        }

        public void Error(string message)
        {
        }

        public void Event(string eventName, string message, LogLevel logLevel = LogLevel.Event)
        {
        }

        public void Exception(Exception ex)
        {
        }

        public void Info(string message)
        {
        }

        public void Instrument(string category, string message)
        {
        }

        public void Warning(string message)
        {
        }
    }
}
