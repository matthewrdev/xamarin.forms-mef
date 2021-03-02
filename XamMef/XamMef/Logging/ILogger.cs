using System;

namespace MFractor.Logging
{
	public interface ILogger
	{
		string Context { get; }

		void Event(string eventName, string message, LogLevel logLevel = LogLevel.Event);

		void Error(string message);
		void Exception(Exception ex);
		void Warning(string message);
		void Info(string message);
		void Debug(string message);
		void Instrument(string category, string message);
	}
}
