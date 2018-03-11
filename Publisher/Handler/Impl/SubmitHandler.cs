using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Kafka.Diff.Common;

namespace Kafka.Diff.Publisher.Handler.Impl
{
    /// <summary>
    /// Main service for submitting diffs (left and right sides).
    /// </summary>
    public class SubmitHandler : ISubmitHandler
    {
        public static readonly IDictionary<string, object> Config = new ConcurrentDictionary<string, object>
        {
            ["bootstrap.servers"] = "localhost:9092", // Default kafka server.
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

        public const string Topic = "diff-topic";
        public const int FlushTimeoutMS = 100;

        private readonly ILogger<SubmitHandler> _logger;
        private readonly ISerializer<SubmitKey> _keySerializer;
        private readonly ISerializer<string> _valueSerializer;
        private readonly ConcurrentDictionary<string, object> _config;

        /// <summary>
        /// Constructor. Initializes injected dependencies.
        /// </summary>
        /// <param name="logger">Logger used by this class.</param>
        /// <param name="keySerializer">A <see cref="SubmitKey"/> key serializer. Used by Kafka.</param>
        /// <param name="valueSerializer">A <see cref="string"/> value serializer. Used by Kafka.</param>
        /// <param name="bootstrapServer">Kafka server. Overrides default 'localhost:9092' value.</param>
        public SubmitHandler(
            ILogger<SubmitHandler> logger,
            ISerializer<SubmitKey> keySerializer,
            ISerializer<string> valueSerializer,
            string bootstrapServer)
        {
            _logger = logger;
            _keySerializer = keySerializer;
            _valueSerializer = valueSerializer;

            _config = new ConcurrentDictionary<string, object>(Config);

            if (!string.IsNullOrWhiteSpace(bootstrapServer))
            {
                _config["bootstrap.servers"] = bootstrapServer;
            }
        }

        /// <summary>
        /// Submits key-value pair to kafka topic.
        /// </summary>
        /// <param name="key">A <see cref="SubmitKey"/> instance.</param>
        /// <param name="value">A <see cref="string"/> instance.</param>
        /// <returns>
        /// A <see cref="Task"/> representing a single asynchronous operation that does not return a value.
        /// </returns>
        /// <remarks>
        /// Right now cannot be unit-tested, since <see cref="Producer{TKey,TValue}"/> methods cannot be mocked.
        /// </remarks>
        public async Task PostAsync(SubmitKey key, string value)
        {
            using (var producer = new Producer<SubmitKey, string>(_config, _keySerializer, _valueSerializer))
            {
                producer.OnError += (_, e)
                    => _logger.Info($"Error: {e}");

                producer.OnLog += (_, m)
                    => _logger.Info($"Name: {m.Name} Level: {m.Level} Facility: {m.Facility} Message: {m.Message}.");

                producer.OnStatistics += (_, s)
                    => _logger.Info($"Statistics: {s}");

                _logger.Info($"Producing item: {key} - {value}.");
                await producer.ProduceAsync(Topic, key, value);

                _logger.Info("Flushing...");
                producer.Flush(FlushTimeoutMS);
                _logger.Info("Flushed.");
            }
        }
    }
}
