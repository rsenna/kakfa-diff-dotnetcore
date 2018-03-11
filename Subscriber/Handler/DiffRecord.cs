using System;
using System.Collections.Generic;

namespace Kafka.Diff.Subscriber.Handler
{
    /// <summary>
    /// Record stored in the LocalDB, containing the diff result.
    /// </summary>
    public class DiffRecord
    {
        /// <summary>
        /// Represents a resulting diff analysis.
        /// Is composed of 0+ <see cref="DiffOffset"/>.
        /// </summary>
        public class DiffAnalysis
        {
            public bool EqualContent { get; set; }
            public bool EqualSize { get; set; }
            public IList<DiffOffset> Offsets { get; set; }
        }

        /// <summary>
        /// A single offset in a <see cref="DiffAnalysis"/> instance.
        /// </summary>
        public class DiffOffset
        {
            public bool IsEqual { get; set; }
            public long Length { get; set; }
        }

        /// <summary>
        /// Primary, unique key.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The left side value to be diffed.
        /// </summary>
        public string Left { get; set; }

        /// <summary>
        /// The right side value to be diffed.
        /// </summary>
        public string Right { get; set; }

        /// <summary>
        /// The result of the diff analysis.
        /// </summary>
        public DiffAnalysis Analysis { get; set; }

        /// <summary>
        /// Indicates if this instance contains both <see cref="Left"/> and <see cref="Right"/>, non null values.
        /// </summary>
        public bool IsComplete => Left != null && Right != null;

        /// <summary>
        /// Used for debugging purposes.
        /// </summary>
        public override string ToString() => $"Id: {Id} Left: {Left} Right: {Right}.";
    }
}
