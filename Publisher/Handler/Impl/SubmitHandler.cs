using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Kafka.Diff.Common;

namespace Kafka.Diff.Publisher.Handler.Impl
{
    public class SubmitHandler : ISubmitHandler
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

        // TODO: inject consts through Autofac
        public const string Topic = "diff-topic";
        public const int FlushTimeoutMS = 100;

        private readonly ILogger<TestProducerHandler> _logger;
        private readonly ISerializer<SubmitKey> _keySerializer;
        private readonly ISerializer<string> _valueSerializer;

        public SubmitHandler(ILogger<TestProducerHandler> logger, ISerializer<SubmitKey> keySerializer, ISerializer<string> valueSerializer)
        {
            _logger = logger;
            _keySerializer = keySerializer;
            _valueSerializer = valueSerializer;
        }

        public async Task Post(SubmitKey key, string value)
        {
            using (var producer = new Producer<SubmitKey, string>(Config, _keySerializer, _valueSerializer))
            {
                // TODO: extract logger setup to factory or decorator
                producer.OnError += (_, e)
                    => _logger.Info($"Error: {e}");

                producer.OnLog += (_, m)
                    =>_logger.Info($"Name: {m.Name} Level: {m.Level} Facility: {m.Facility} Message: {m.Message}.");

                producer.OnStatistics += (_, s)
                    => _logger.Info($"Statistics: {s}");

                _logger.Info($"Producing item: {key} - {value}.");
                await producer.ProduceAsync(Topic, key, value);

                _logger.Info("Flushing...");
                producer.Flush(FlushTimeoutMS);
            }

            _logger.Info("Completed.");
        }
    }
}
