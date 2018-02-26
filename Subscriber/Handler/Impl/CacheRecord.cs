using System;

namespace Kafka.Diff.Subscriber.Handler.Impl
{
    public class CacheRecord
    {
        public Guid Id { get; set; }

        public string Left { get; set; }
        public string Right { get; set; }
        public string Diff { get; set; }

        public bool IsFull => Left != null && Right != null;

        public override string ToString() => $"Id: {Id} Left: {Left} Right: {Right} Diff: {Diff}.";
    }
}
