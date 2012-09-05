using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ByoBaby.Rest.Helpers
{
    public static class Log
    {
        public static void LogInformation(string message, params object[] args)
        {
            Trace.TraceInformation(message, args);
        }

        public static void LogWarning(string message, params object[] args)
        {
            Trace.TraceWarning(message, args);
        }

        public static void LogStart()
        {

        }

        public static void LogStop()
        {

        }

        public static void LogVerbose()
        {

        }

        public static void LogError()
        {

        }
    }
}