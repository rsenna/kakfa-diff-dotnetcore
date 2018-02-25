using Kakfka.Diff.Subscriber.Handler.Impl;

namespace Kakfka.Diff.Subscriber.Handler
{
    public interface IDiffRepository
    {
        void Save(CacheRecord record);
        CacheRecord Load(string id);
    }
}
