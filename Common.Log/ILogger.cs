namespace Kafka.Diff.Common.Log
{
    public interface ILogger<in T>
    {
        void Info(string message);
    }
}