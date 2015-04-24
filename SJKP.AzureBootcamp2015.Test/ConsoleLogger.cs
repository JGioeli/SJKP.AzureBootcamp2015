using System;
using System.Diagnostics;
using Microsoft.DataFactories.Runtime;

namespace SJKP.AzureBootcamp2015.Test
{
    internal class ConsoleLogger : IActivityLogger
    {
        public ConsoleLogger()
        {
        }

        public void Write(TraceEventType messageType, string format, params object[] args)
        {
            Console.Write(format, args);
        }
    }
}