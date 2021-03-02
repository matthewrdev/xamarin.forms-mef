using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MFractor.Logging
{
    public class NonBlockingConsoleLogger : LogWriter
    {
        readonly ConcurrentQueue<Tuple<LogLevel, string>> outputList;
        readonly Thread outputThread;
        readonly AutoResetEvent @event;
        readonly object exitLock = new object();
        bool shouldExit = false;

        bool ShouldExit
        {
            get
            {
                lock (exitLock)
                {
                    return shouldExit;
                }
            }
            set
            {
                lock (exitLock)
                {
                    shouldExit = value;
                }
            }
        }

        public string Render(string type, string message, LogLevel logLevel = LogLevel.Event)
        {
            return string.Format("{0} [{1} : {2}] - {3}", DateTime.Now, Context, type, message);

        }

        public NonBlockingConsoleLogger(string context)
            : base(context)
        {
            outputList = new ConcurrentQueue<Tuple<LogLevel, string>>();
            outputThread = new Thread(new ThreadStart(this.OutputLogQueue));
            @event = new AutoResetEvent(false);

            outputThread.Start();
        }

        public override void Event(string type, string message, LogLevel logLevel = LogLevel.Event)
        {
            Write(logLevel, Render(type, message, logLevel));
        }

        public override void Exception(Exception ex)
        {
            Write(LogLevel.Error, (Render("Exception", ex.ToString(), LogLevel.Error)));
        }

        public void Dispose()
        {
            ShouldExit = true;
            if (!outputThread.Join(10))
            {
                outputThread.Abort();
            }
        }

        public void Write(LogLevel logLevel, string output)
        {
            outputList.Enqueue(new Tuple<LogLevel, string>(logLevel, output));
            @event.Set();
        }

        void OutputLogQueue()
        {
            while (!ShouldExit)
            {
                @event.WaitOne();
                while (outputList.IsEmpty == false && !ShouldExit)
                {
                    Tuple<LogLevel, string> output = null;
                    if (outputList.TryDequeue(out output))
                    {
                        switch (output.Item1)
                        {
                            case LogLevel.Error:
                                Console.ForegroundColor = ConsoleColor.Red;
                                break;
                            case LogLevel.Warning:
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                break;
                            case LogLevel.Event:
                                Console.ForegroundColor = ConsoleColor.Blue;
                                break;
                            default:
                                Console.ForegroundColor = ConsoleColor.White;
                                break;
                        }

                        Console.WriteLine(output.Item2);
                    }
                }
            }
        }
    }
}

