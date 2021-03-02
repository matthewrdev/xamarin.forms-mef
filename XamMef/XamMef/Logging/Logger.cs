using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace MFractor.Logging
{
    public sealed class Logger
    {
        public static readonly Logger Instance = new Logger();
        readonly object factoryLock = new object();
        ILoggerFactory factory = new ConsoleLoggerFactory();

        public ILoggerFactory Factory
        {
            get
            {
                lock (factoryLock)
                {
                    return factory;
                }
            }
            set
            {
                lock (factoryLock)
                {
                    factory = value;
                }
            }
        }

        /// <summary>
        /// Creates a new <see cref="ILogger"/>, capturing the name of the file where is was created as the logger context..
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ILogger Create([CallerFilePath]string context = "")
        {
            if (Instance == null)
            {
                return null;
            }

            if (Instance.Factory == null)
            {
                return null;
            }

            var fileInfo = new FileInfo(context);

            return Instance.Factory.Create(fileInfo.Name);
        }

        public void Close()
        {
            Factory.Dispose();
            Factory = null;
        }
    }
}

