using System;
using NtpServiceLibrary;

namespace NtpServiceConsole
{

    internal class ConsoleLogger : ILogger
    {
        readonly string _serviceName;
        public ConsoleLogger(string serviceName)
        {
            _serviceName = serviceName;
        }
        public void Start()
        {
        }

        public void Write(string message)
        {
            Console.WriteLine(string.Format("{0}: {1}", _serviceName, message));
        }

        public void Write(string format, params object[] parameters)
        {
            Console.WriteLine(string.Format("{0}: {1}", _serviceName, string.Format(format, parameters)));
        }

    }
}
