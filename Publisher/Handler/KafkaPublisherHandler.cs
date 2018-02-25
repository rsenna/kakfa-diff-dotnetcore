using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Kafka.Diff.Common.Log;

namespace Kafka.Diff.Publisher.Handler
{
    public class KafkaPublisherHandler : IPublisherHandler
    {
        public static readonly IDictionary<string, object> Config = new ConcurrentDictionary<string, object>
        {
            ["bootstrap.servers"] = "localhost:9092",
            // ["debug"] = "all",
            ["retries"] = 0,
            ["batch.num.messages"] = 1,
            ["socket.blocking.max.ms"] = 1,
            ["socket.nagle.disable"] = true,
            ["queue.buffering.max.ms"] = 0,
            ["default.topic.config"] = new Dictionary<string, object>
            {
                ["acks"] = 1
            }
        };

        public static readonly StringSerializer UTF8Serializer = new StringSerializer(Encoding.UTF8);

        private readonly ILogger<KafkaPublisherHandler> _logger;

        public KafkaPublisherHandler(ILogger<KafkaPublisherHandler> logger)
        {
            _logger = logger;
        }

        public async Task Test(ICollection<string> items)
        {
            using (var producer = new Producer<Null, string>(Config, null, UTF8Serializer))
            {
                producer.OnError += (_, e)
                    => _logger.Info($"Error: {e}");

                producer.OnLog += (_, m)
                    =>_logger.Info($"Name: {m.Name} Level: {m.Level} Facility: {m.Facility} Message: {m.Message}.");

                producer.OnStatistics += (_, s)
                    => _logger.Info($"Statistics: {s}");

                LogGroups(producer);
                LogMetadata(producer);

                foreach (var it in items)
                {
                    _logger.Info($"Producing item: {it}.");
                    await producer.ProduceAsync("hello-topic", null, it);
                }

                _logger.Info("Flushing...");
                producer.Flush(100);
            }

            _logger.Info("Completed.");
        }

        private void LogGroups(Producer<Null, string> producer)
        {
            _logger.Info("Consumer Groups:");

            var groups = producer.ListGroups(TimeSpan.FromSeconds(10));

            foreach (var g in groups)
            {
                _logger.Info($"  Group: {g.Group} {g.Error} {g.State}");
                _logger.Info($"  Broker: {g.Broker.BrokerId} {g.Broker.Host}:{g.Broker.Port}");
                _logger.Info($"  Protocol: {g.ProtocolType} {g.Protocol}");
                _logger.Info($"  Members:");
                foreach (var m in g.Members)
                {
                    _logger.Info($"    {m.MemberId} {m.ClientId} {m.ClientHost}");
                    _logger.Info($"    Metadata: {m.MemberMetadata.Length} bytes");
                    _logger.Info($"    Assignment: {m.MemberAssignment.Length} bytes");
                }
            }
        }

        private void LogMetadata(Producer<Null, string> producer)
        {
            _logger.Info("Consumer metadata:");

            var meta = producer.GetMetadata(true, null);

            _logger.Info($"{meta.OriginatingBrokerId} {meta.OriginatingBrokerName}");

            meta.Brokers.ForEach(broker =>
                _logger.Info($"Broker: {broker.BrokerId} {broker.Host}:{broker.Port}"));

            meta.Topics.ForEach(topic =>
            {
                _logger.Info($"Topic: {topic.Topic} {topic.Error}");
                topic.Partitions.ForEach(partition =>
                {
                    _logger.Info($"  Partition: {partition.PartitionId}");
                    _logger.Info($"    Replicas: {partition.Replicas}");
                    _logger.Info($"    InSyncReplicas: {partition.InSyncReplicas}");
                });
            });
        }
    }
}
