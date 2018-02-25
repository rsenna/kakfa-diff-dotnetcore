using System;
using System.Collections.Generic;
using Confluent.Kafka.Serialization;
using Kafka.Diff.Common.Log;
using Kafka.Diff.Publisher.Handler;

namespace Kafka.Diff.Publisher.Serializer
{
    public class SubmitKeySerializer : ISerializer<SubmitKey>
    {
        private static readonly StringSerializer InnerSerializer = new UTF8Serializer();

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
