namespace GCop.Common.Core
{
    using System;
    using System.Diagnostics;

    public static class Logger
    {
        const string GCopLogFileName = "GCopLog.txt";

        public static void Log(Exception exception, string actualStackTrace = null)
        {

            LogToOutputWindow(exception, actualStackTrace);
        }

        static void LogToOutputWindow(Exception exception, string actualStackTrace = null)
        {
            Debug.WriteLine($"GCOP_ERROR:{Environment.NewLine}{exception.ToFullMessage()}{actualStackTrace}");
        }
    }
}
