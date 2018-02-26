using Kafka.Diff.Subscriber.Handler.Impl;

namespace Kafka.Diff.Subscriber.Handler
{
    public interface IDiffGenerator
    {
        string GetDiff(CacheRecord cacheRecord);
    }
}
