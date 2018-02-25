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
            ["bootstrap.servers"] = "localhost:9092"
        };

        public static readonly StringSerializer UTF8Serializer = new StringSerializer(Encoding.UTF8);

        private readonly ILogger<KafkaPublisherHandler> _logger;

        public KafkaPublisherHandler(ILogger<KafkaPublisherHandler> logger)
        {
            _logger = logger;
        }

        public Task Test(ICollection<string> items)
        {
            using (var producer = new Producer<Null, string>(Config, null, UTF8Serializer))
            {
                producer.OnError += (_, e)
                    => _logger.Info($"Error: {e}");

                producer.OnLog += (_, m)
                    =>_logger.Info($"Name: {m.Name} Level: {m.Level} Facility: {m.Facility} Message: {m.Message}.");

                producer.OnStatistics += (_, s)
                    => _logger.Info($"Statistics: {s}");

                foreach (var it in items)
                {
                    _logger.Info($"Producing item: {it}.");
                    producer.ProduceAsync("hello-topic", null, it);
                }

                _logger.Info("Flushing...");
                producer.Flush(100);
            }

            _logger.Info("Completed.");
            return Task.CompletedTask;
        }
    }
}
