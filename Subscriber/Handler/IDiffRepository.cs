using System;
using Kafka.Diff.Subscriber.Handler.Impl;

namespace Kafka.Diff.Subscriber.Handler
{
    public interface IDiffRepository
    {
        void Save(CacheRecord record);
        CacheRecord Load(Guid id);
    }
}
