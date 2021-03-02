using System;

namespace MFractor.Logging
{
    public abstract class LogWriter : ILogger
    {
        public LogWriter(string context)
        {
            Context = context;
        }

        public void Error(string message)
        {
            Event("error", message, LogLevel.Error);
        }

        public void Warning(string message)
        {
            Event("warning", message, LogLevel.Warning);
        }

        public void Info(string message)
        {
            Event("info", message, LogLevel.Info);
        }

        public void Debug(string message)
        {
            Event("debug", message, LogLevel.Debug);
        }

        public void Instrument(string category, string message)
        {
            Event("instrument[" + category + "]", message, LogLevel.Instrument);
        }

        public string Context
        {
            get;
        }

        public abstract void Event(string type, string message, LogLevel logLevel = LogLevel.Event);

        public abstract void Exception(Exception ex);
    }
}

