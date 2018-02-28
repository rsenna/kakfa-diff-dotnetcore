using System;
using System.Collections.Generic;
using System.IO;
using Confluent.Kafka;

namespace Kafka.Diff.Subscriber.Handler.Impl
{
    public class DiffGenerator : IDiffGenerator
    {
        public void RefreshDiff(CacheRecord cacheRecord)
        {
            if (cacheRecord == null || !cacheRecord.IsFull)
            {
                return;
            }

            var left = Convert.FromBase64String(cacheRecord.Left);
            var right = Convert.FromBase64String(cacheRecord.Right);

            var equalSize = cacheRecord.Analysis.EqualSize = left.Length == right.Length;
            cacheRecord.Analysis.EqualContent = !equalSize;

            if (!equalSize)
            {
                return;
            }

            // This is a naive implementation of a diff. It will search both streams at the same time,
            // at each byte, and return informations about equal and distinct windows, at THE SAME POSITION.
            // For a proper difference algorithm in C#, refer to http://www.mathertel.de/Diff/

            CacheRecord.Offset currOffset = null;
            var offsets = new List<CacheRecord.Offset>();

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
                        currOffset = new CacheRecord.Offset {IsEqual = inSync};
                    }

                    currOffset.Length++;
                }
            }

            cacheRecord.Analysis.EqualContent = offsets.Count == 1 && offsets[0].IsEqual;
        }
    }
}
