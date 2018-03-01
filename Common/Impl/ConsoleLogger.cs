using System;

namespace Kafka.Diff.Common.Impl
{
    internal class ConsoleLogger<T> : ILogger<T>
    {
        private readonly string _section;

        public ConsoleLogger()
        {
            _section = typeof(T).FullName;
        }

        public void Info(string message)
        {
            Console.WriteLine($"Info: {_section}: {message}");
        }

        public void Error(Exception ex)
        {
            Console.WriteLine($"Error: {_section}: {ex.Message}");
        }
    }
}
