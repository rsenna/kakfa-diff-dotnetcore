using System;
using System.Collections.Generic;
using Confluent.Kafka.Serialization;
using Kafka.Diff.Common;

namespace Kafka.Diff.Subscriber.Deserializer
{
    public class SubmitKeyDeserializer : IDeserializer<SubmitKey>
    {
        private static readonly StringDeserializer InnerDeserializer = new UTF8Deserializer();

        public SubmitKey Deserialize(string topic, byte[] data)
        {
            var text = InnerDeserializer.Deserialize(topic, data);
            var result = SubmitKey.FromString(text);
            return result;
        }

        public IEnumerable<KeyValuePair<string, object>> Configure(IEnumerable<KeyValuePair<string, object>> config, bool isKey)
        {
            if (!isKey)
            {
                throw new InvalidOperationException($"{nameof(isKey)} must be true.");
            }

            return InnerDeserializer.Configure(config, true);
        }
    }
}
