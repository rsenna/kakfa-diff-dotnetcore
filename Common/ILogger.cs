using System;

namespace Kafka.Diff.Common
{
    public interface ILogger<in T>
    {
        void Info(string message);
        void Error(Exception ex);
    }
}