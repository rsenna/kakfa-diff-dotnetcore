using System;
using Kafka.Diff.Subscriber.Handler.Impl;

namespace Kafka.Diff.Subscriber.Handler
{
    public interface IDiffRepository
    {
        void Save(DiffRecord record);
        DiffRecord Load(Guid id);
    }
}
