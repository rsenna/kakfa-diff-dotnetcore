using Kakfka.Diff.Subscriber.Handler.Impl;

namespace Kakfka.Diff.Subscriber.Handler
{
    public interface IDiffGenerator
    {
        string GetDiff(CacheRecord cacheRecord);
    }
}
