namespace Kafka.Diff.Common
{
    public interface ILogger<in T>
    {
        void Info(string message);
    }
}