using System;
using System.Collections.Generic;

namespace Kafka.Diff.Subscriber.Handler.Impl
{
    public class CacheRecord
    {
        public class RecordAnalysis
        {
            public bool EqualContent { get; set; }
            public bool EqualSize { get; set; }
            public IEnumerable<Offset> Offsets { get; set; }
        }

        public class Offset
        {
            public bool IsEqual { get; set; }
            public long Length { get; set; }
        }

        public Guid Id { get; set; }

        public string Left { get; set; }
        public string Right { get; set; }
        public RecordAnalysis Analysis { get; set; }

        public bool IsFull => Left != null && Right != null;

        public override string ToString() => $"Id: {Id} Left: {Left} Right: {Right}.";
    }
}
