using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Kafka.Diff.Common.Log;

namespace Kakfka.Diff.Subscriber.Handler.Impl
{
    public class TestConsumerSubscribeHandler : ITestConsumerHandler
    {
        public static readonly IDictionary<string, object> Config = new ConcurrentDictionary<string, object>
        {
            // TODO: merge config with default in Factory
            ["group.id"] = "kafka-diff",
            ["bootstrap.servers"] = "localhost:9092",
            ["enable.auto.commit"] = true,
            ["auto.commit.interval.ms"] = 1000,
            ["statistics.interval.ms"] = 10000,
            ["default.topic.config"] = new Dictionary<string, object>
            {
                ["auto.offset.reset"] = "smallest"
            }
        };

        public static readonly StringDeserializer UTF8Deserializer = new StringDeserializer(Encoding.UTF8);

        private readonly ILogger<TestConsumerAssignHandler> _logger;
        private readonly Consumer<Ignore, string> _consumer;

        public TestConsumerSubscribeHandler(ILogger<TestConsumerAssignHandler> logger, IKafkaConsumerFactory<Ignore, string> consumerFactory)
        {
            _logger = logger;
            _consumer = consumerFactory.Create(Config, null, UTF8Deserializer);

            // TODO: remove hardcoded topic
            _consumer.Subscribe("hello-topic");
            _logger.Info($"Subscribed to: [{string.Join(", ", _consumer.Subscription)}].");
        }

        public async Task<IEnumerable<string>> Test(int take)
        {
            var messages = ConsumeMessages(take).ToList();

            if (messages.Any())
            {
                _logger.Info($"Committing offset...");

                var committedOffsets = await _consumer.CommitAsync(messages.Last());

                _logger.Info($"Committed offset: {committedOffsets}");
            }

            return messages
                .Select(m => m.Value)
                .ToList();
        }

        public IEnumerable<Message<Ignore, string>> ConsumeMessages(int take)
        {
            for (var i = 0; i < take; i++)
            {
                if (!_consumer.Consume(out var message, millisecondsTimeout: 2000))
                {
                    _logger.Info($"Did not consume any message.");
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
