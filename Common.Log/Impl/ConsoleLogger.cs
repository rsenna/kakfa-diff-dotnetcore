﻿using System;

namespace Kafka.Diff.Common.Log.Impl
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
            Console.WriteLine($"{_section}: {message}");
        }
    }
}