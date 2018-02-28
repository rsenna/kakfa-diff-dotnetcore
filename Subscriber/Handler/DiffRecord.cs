using System;
using System.Collections.Generic;

namespace Kafka.Diff.Subscriber.Handler
{
    public class DiffRecord
    {
        public class DiffAnalysis
        {
            public bool EqualContent { get; set; }
            public bool EqualSize { get; set; }
            public IList<DiffOffset> Offsets { get; set; }
        }

        public class DiffOffset
        {
            public bool IsEqual { get; set; }
            public long Length { get; set; }
        }

        public Guid Id { get; set; }

        public string Left { get; set; }
        public string Right { get; set; }
        public DiffAnalysis Analysis { get; set; }

        public bool IsComplete => Left != null && Right != null;

        public override string ToString() => $"Id: {Id} Left: {Left} Right: {Right}.";
    }
}
