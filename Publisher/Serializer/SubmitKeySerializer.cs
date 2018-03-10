using System;
using System.Collections.Generic;
using Confluent.Kafka.Serialization;
using Kafka.Diff.Common;
using Kafka.Diff.Publisher.Handler;

namespace Kafka.Diff.Publisher.Serializer
{
    /// <summary>
    /// Serializes <see cref="SubmitKey"/> into a string. Used by Kafka.
    /// </summary>
    public class SubmitKeySerializer : ISerializer<SubmitKey>
    {
        public static readonly StringSerializer InnerSerializer = new UTF8Serializer();

        public byte[] Serialize(string topic, SubmitKey data) => InnerSerializer.Serialize(topic, data.ToString());

        public IEnumerable<KeyValuePair<string, object>> Configure(IEnumerable<KeyValuePair<string, object>> config, bool isKey)
        {
            if (!isKey)
            {
                throw new InvalidOperationException($"{nameof(isKey)} must be true.");
            }

            return InnerSerializer.Configure(config, true);
        }
    }
}
