using System;
using System.Collections.Concurrent;
using System.Threading;
using System.IO;

namespace MFractor.Logging
{
    public class LogFileWriter : IDisposable
    {
        readonly ConcurrentQueue<string> outputList;
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

        public readonly string SessionFile;

        public LogFileWriter(string sessionFile)
        {
            SessionFile = sessionFile;

            outputList = new ConcurrentQueue<string>();
            outputThread = new Thread(new ThreadStart(this.OutputLogQueue));
            @event = new AutoResetEvent(false);

            outputThread.Start();
        }

        public void Dispose()
        {
            ShouldExit = true;
            @event.Set();
            WriteToFile("Close log");
            if (!outputThread.Join(10))
            {
                outputThread.Abort();
            }
        }

        public void WriteToFile(string output)
        {
            outputList.Enqueue(output);
            @event.Set();
        }

        void OutputLogQueue()
        {
            while (!ShouldExit)
            {
                @event.WaitOne();
                while (outputList.IsEmpty == false && !ShouldExit)
                {
                    if (outputList.TryDequeue(out var output))
                    {
                        File.AppendAllText(SessionFile, output);
                    }
                }
            }
        }
    }
}

