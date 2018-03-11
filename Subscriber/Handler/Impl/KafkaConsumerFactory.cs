﻿using System.Collections.Generic;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Kafka.Diff.Common;

namespace Kafka.Diff.Subscriber.Handler.Impl
{
    /// <summary>
    /// Kafka consumer factory.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class KafkaConsumerFactory<TKey, TValue> : IKafkaConsumerFactory<TKey, TValue>
    {
        private readonly ILogger<KafkaConsumerFactory<TKey, TValue>> _logger;

        public KafkaConsumerFactory(ILogger<KafkaConsumerFactory<TKey, TValue>> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Creates a new <see cref="Consumer{TKey,TValue}"/> with the specified parameters, using a pre-defined log
        /// configuration.
        /// </summary>
        /// <param name="config">librdkafka configuration parameters.
        /// </param>
        /// <param name="keyDeserializer">
        /// An <see cref="IDeserializer{T}"/> implementation instance for deserializing keys.
        /// </param>
        /// <param name="valueDeserializer">
        /// An <see cref="IDeserializer{T}"/> implementation instance for deserializing values.
        /// </param>
        /// <returns>
        /// A new <see cref="Consumer{TKey,TValue}"/> with the specified parameters & configuration.
        /// </returns>
        /// <remarks>
        /// Refer to <a href="https://github.com/edenhill/librdkafka/blob/master/CONFIGURATION.md">this link</a> for
        /// more info about kafka's configuration options.
        /// </remarks>
        public Consumer<TKey, TValue> Create(IDictionary<string, object> config, IDeserializer<TKey> keyDeserializer, IDeserializer<TValue> valueDeserializer)
        {
            var consumer = new Consumer<TKey, TValue>(config, keyDeserializer, valueDeserializer);

            consumer.OnPartitionEOF += (_, end)
                => _logger.Info($"Reached end of topic {end.Topic} partition {end.Partition}, next message will be at offset {end.Offset}");

            consumer.OnError += (_, error)
                => _logger.Info($"Error: {error}");

            consumer.OnConsumeError += (_, error)
                => _logger.Info($"Consume error: {error}");

            consumer.OnOffsetsCommitted += (_, commit) =>
            {
                _logger.Info($"[{string.Join(", ", commit.Offsets)}]");

                if (commit.Error)
                {
                    _logger.Info($"Failed to commit offsets: {commit.Error}");
                }

                _logger.Info($"Successfully committed offsets: [{string.Join(", ", commit.Offsets)}]");
            };

            consumer.OnPartitionsAssigned += (_, partitions) =>
            {
                _logger.Info($"Assigned partitions: [{string.Join(", ", partitions)}], member id: {consumer.MemberId}");
                consumer.Assign(partitions);
            };

            consumer.OnPartitionsRevoked += (_, partitions) =>
            {
                _logger.Info($"Revoked partitions: [{string.Join(", ", partitions)}]");
                consumer.Unassign();
            };

            consumer.OnStatistics += (_, s)
                => _logger.Info($"Statistics: {s}");

            return consumer;
        }
    }
}
