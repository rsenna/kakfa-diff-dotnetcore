using System.Collections.Generic;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Kafka.Diff.Common.Log;

namespace Kakfka.Diff.Subscriber.Handler.Impl
{
    public class KafkaConsumerFactory<TKey, TValue> : IKafkaConsumerFactory<TKey, TValue>
    {
        private readonly ILogger<TestConsumerAssignHandler> _logger;

        public KafkaConsumerFactory(ILogger<TestConsumerAssignHandler> logger)
        {
            _logger = logger;
        }

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
