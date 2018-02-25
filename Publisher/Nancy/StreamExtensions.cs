﻿using System.IO;
using System.Text;

namespace Kafka.Diff.Publisher.Nancy
{
    public static class StreamExtensions
    {
        public static string AsString(this Stream stream, Encoding encoding = null)
        {
            using (var streamReader = new StreamReader(stream, encoding ?? Encoding.UTF8))
            {
                var position = stream.Position;
                stream.Position = 0L;
                var end = streamReader.ReadToEnd();
                stream.Position = position;

                return end;
            }
        }
    }
}