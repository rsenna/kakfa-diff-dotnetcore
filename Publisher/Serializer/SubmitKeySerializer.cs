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

        /// <summary>Serialize an instance of <see cref="SubmitKey"/> to a byte array.</summary>
        /// <param name="topic">The topic associated wih the data.</param>
        /// <param name="data">A <see cref="SubmitKey"/> to serialize.</param>
        /// <returns><paramref name="data" /> serialized as a byte array.</returns>
        public byte[] Serialize(string topic, SubmitKey data) =>
            InnerSerializer.Serialize(topic, data.ToString());

        /// <summary>
        /// Configure the serializer using relevant configuration parameter(s) in <paramref name="config" /> (if present)
        /// </summary>
        /// <param name="config">A collection containing configuration parameter(s) relevant to this serializer.</param>
        /// <param name="isKey">Must be true - indicates this class is used to serialize keys. </param>
        /// <returns>An updated configuration collection.</returns>
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
