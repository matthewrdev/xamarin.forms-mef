namespace MFractor.Logging
{
    public class StubLoggerFactory : ILoggerFactory
    {
        public StubLoggerFactory()
        {
        }

        public ILogger Create(string context)
        {
            return new StubLogger(context);
        }

        public void Dispose()
        {
        }
    }
}
