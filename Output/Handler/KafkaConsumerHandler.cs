using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Kafka.Diff.Common.Log;

namespace Kakfka.Diff.Output.Handler
{
    public class KafkaConsumerHandler : IConsumerHandler
    {
        public static readonly IDictionary<string, object> Config = new ConcurrentDictionary<string, object>
        {
            ["group.id"] = "sample-consumer",
            ["bootstrap.servers"] = "localhost:9092",
            ["enable.auto.commit"] = false
        };

        public static readonly StringDeserializer DefaultDeserializer = new StringDeserializer(Encoding.UTF8);

        private readonly ILogger<KafkaConsumerHandler> _logger;
        private readonly Consumer<Ignore, string> _consumer;

        public KafkaConsumerHandler(ILogger<KafkaConsumerHandler> logger)
        {
            _logger = logger;
            _consumer = new Consumer<Ignore, string>(Config, null, DefaultDeserializer);

            _consumer.OnPartitionEOF += (_, end)
                => _logger.Info($"Reached end of topic {end.Topic} partition {end.Partition}, next message will be at offset {end.Offset}");

            _consumer.OnError += (_, error)
                => _logger.Info($"Error: {error}");

            _consumer.OnConsumeError += (_, error)
                => _logger.Info($"Consume error: {error}");

            _consumer.OnPartitionsAssigned += (_, partitions) =>
            {
                _logger.Info($"Assigned partitions: [{string.Join(", ", partitions)}], member id: {_consumer.MemberId}");
                _consumer.Assign(partitions);
            };

            _consumer.OnPartitionsRevoked += (_, partitions) =>
            {
                _logger.Info($"Revoked partitions: [{string.Join(", ", partitions)}]");
                _consumer.Unassign();
            };

            _consumer.OnStatistics += (_, json)
                => _logger.Info($"Statistics: {json}");

            _consumer.Subscribe("hello-topic");
        }

        public async Task<IEnumerable<string>> Test(int take)
        {
            var messages = ConsumeMessages(take).ToList();

            if (messages.Any())
            {
                // In Kafka, by commiting the last consumed message we signal that all messages have being processed successfully.
                await _consumer.CommitAsync(messages.Last());
            }

            return messages
                .Select(m => m.Value)
                .ToList();
        }

        public IEnumerable<Message<Ignore, string>> ConsumeMessages(int take)
        {
            for (var i = 0; i < take; i++)
            {
                if (!_consumer.Consume(out var message, millisecondsTimeout: 100))
                {
                    continue;
                }

                _logger.Info($"Topic: {message.Topic} Partition: {message.Partition} Offset: {message.Offset} {message.Value}");

                yield return message;
            }
        }

        public void Dispose()
        {
            _consumer?.Dispose();
        }
    }
}
