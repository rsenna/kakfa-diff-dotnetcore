﻿using System;
using System.Collections.Generic;
using Confluent.Kafka.Serialization;
using Kafka.Diff.Common;

namespace Kafka.Diff.Subscriber.Deserializer
{
    /// <summary>
    /// Deserializes <see cref="SubmitKey"/> from a string. Used by Kafka.
    /// </summary>
    public class SubmitKeyDeserializer : IDeserializer<SubmitKey>
    {
        private static readonly StringDeserializer InnerDeserializer = new UTF8Deserializer();

        /// <summary>
        /// Deserialize a byte array to an instance of <see cref="SubmitKey"/>.
        /// </summary>
        /// <param name="topic">The topic associated wih the data.</param>
        /// <param name="data">The serialized representation of an instance of <see cref="SubmitKey"/>.</param>
        /// <returns>A <see cref="SubmitKey"/> instance.</returns>
        public SubmitKey Deserialize(string topic, byte[] data)
        {
            var text = InnerDeserializer.Deserialize(topic, data);
            var result = SubmitKey.FromString(text);
            return result;
        }

        /// <summary>
        /// Configure the deserializer using relevant configuration parameter(s) in <paramref name="config" /> (if present)
        /// </summary>
        /// <param name="config">A collection containing configuration parameter(s) relevant to this deserializer.</param>
        /// <param name="isKey">Must be true - indicates this class is used to serialize keys. </param>
        /// <returns>An updated configuration collection.</returns>
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
