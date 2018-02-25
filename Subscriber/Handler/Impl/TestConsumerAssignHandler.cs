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
    public class TestConsumerAssignHandler : ITestConsumerHandler
    {
        public static readonly IDictionary<string, object> Config = new ConcurrentDictionary<string, object>
        {
            ["group.id"] = "kafka-diff",
            ["bootstrap.servers"] = "localhost:9092",
            ["enable.auto.commit"] = false
        };

        public static readonly StringDeserializer UTF8Deserializer = new StringDeserializer(Encoding.UTF8);

        private readonly ILogger<TestConsumerAssignHandler> _logger;
        private readonly Consumer<Ignore, string> _consumer;

        public TestConsumerAssignHandler(ILogger<TestConsumerAssignHandler> logger, IKafkaConsumerFactory<Ignore, string> consumerFactory)
        {
            _logger = logger;
            _consumer = consumerFactory.Create(Config, null, UTF8Deserializer);

            _consumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset("hello-topic", 0, 0) });
        }

        public Task<IEnumerable<string>> Test(int take)
        {
            var messages = ConsumeMessages(take).ToList();

            return Task.FromResult(messages
                .Select(m => m.Value)
                .ToList()
                .AsEnumerable());
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
