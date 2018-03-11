using System;
using System.Collections.Generic;
using System.IO;

namespace Kafka.Diff.Subscriber.Handler.Impl
{
    /// <summary>
    /// Generates diffs in a <see cref="DiffRecord"/>
    /// </summary>
    public class DiffGenerator : IDiffGenerator
    {
        /// <summary>
        /// Updates diff results into a <see cref="DiffRecord"/>, regardless if it already exists or not.
        /// </summary>
        /// <param name="diffRecord">The <see cref="DiffRecord"/> instance to be updated.</param>
        /// <remarks>
        /// Pre-conditions (otherwise this method will silently exit):<br/>
        /// - <see cref="DiffRecord.IsComplete"/> == true<br/>
        /// - <see cref="DiffRecord.Left"/> and <see cref="DiffRecord.Right"/> must have same length.
        /// </remarks>
        public void RefreshDiff(DiffRecord diffRecord)
        {
            if (diffRecord == null)
            {
                return;
            }

            diffRecord.Analysis = null;

            if (!diffRecord.IsComplete)
            {
                return;
            }

            var left = Convert.FromBase64String(diffRecord.Left);
            var right = Convert.FromBase64String(diffRecord.Right);

            var analysis = new DiffRecord.DiffAnalysis();

            var equalSize = analysis.EqualSize = left.Length == right.Length;
            analysis.EqualContent = !equalSize;

            if (!equalSize)
            {
                return;
            }

            // This is a naive implementation of a diff. It will search both streams at the same time,
            // at each byte, and return informations about equal and distinct windows, at THE SAME POSITION.
            // For a proper difference algorithm in C#, refer to http://www.mathertel.de/Diff/

            DiffRecord.DiffOffset currOffset = null;
            var offsets = new List<DiffRecord.DiffOffset>();

            using (var leftStream = new MemoryStream(left))
            using (var leftReader = new StreamReader(leftStream))
            using (var rightStream = new MemoryStream(right))
            using (var rightReader = new StreamReader(rightStream))
            {
                while (true)
                {
                    var l = leftReader.Read();
                    var r = rightReader.Read();

                    if (l < 0 || r < 0)
                    {
                        if (currOffset != null)
                        {
                            offsets.Add(currOffset);
                        }

                        break;
                    }

                    var inSync = l == r;

                    if (currOffset != null && (currOffset.IsEqual != inSync && currOffset.Length > 0))
                    {
                        offsets.Add(currOffset);
                        currOffset = null;
                    }

                    if (currOffset == null)
                    {
                        currOffset = new DiffRecord.DiffOffset {IsEqual = inSync};
                    }

                    currOffset.Length++;
                }
            }

            analysis.EqualContent = offsets.Count == 1 && offsets[0].IsEqual;

            if (!analysis.EqualContent)
            {
                analysis.Offsets = offsets;
            }

            diffRecord.Analysis = analysis;
        }
    }
}
