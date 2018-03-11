using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Kafka.Diff.Common;

namespace Kafka.Diff.Subscriber.Handler.Impl
{
    /// <summary>
    /// Listener used to process messages in the `diff-topic` queued.
    /// </summary>
    public sealed class TopicListener : ITopicListener, IDisposable
    {
        public static readonly IDictionary<string, object> Config = new ConcurrentDictionary<string, object>
        {
            ["bootstrap.servers"] = "localhost:9092", // Default kafka server.
            ["group.id"] = "kafka-diff",
            ["enable.auto.commit"] = false
        };

        public const string Topic = "diff-topic";
        public const int ConsumeTimeoutMS = 5000;

        private readonly IDiffGenerator _diffGenerator;
        private readonly IDiffRepository _diffRepository;
        private readonly ILogger<TopicListener> _logger;
        private readonly Consumer<SubmitKey, string> _consumer;

        /// <summary>
        /// Constructor. Initializes the <see cref="_consumer"/> instance.
        /// </summary>
        /// <param name="consumerFactory">
        /// A <see cref="IKafkaConsumerFactory{TKey,TValue}"/> using <see cref="SubmitKey"/> keys and <see cref="string"/> values</param>
        /// <param name="diffGenerator">A <see cref="IDiffGenerator"/>. Used to generate diffs from left and right values.</param>
        /// <param name="diffRepository">A <see cref="IDiffRepository"/>. Used to persist <see cref="DiffRecord"/>s.</param>
        /// <param name="keyDeserializer">A <see cref="IDeserializer{T}"/>, used to deserialize <see cref="SubmitKey"/>s.</param>
        /// <param name="valueDeserializer">A <see cref="IDeserializer{T}"/>, used to deserialize <see cref="string"/> values.</param>
        /// <param name="logger">A <see cref="ILogger{T}"/> for this class.</param>
        /// <param name="bootstrapServer">Kafka server. Overrides default 'localhost:9092' value.</param>
        public TopicListener(
            IKafkaConsumerFactory<SubmitKey, string> consumerFactory,
            IDiffGenerator diffGenerator,
            IDiffRepository diffRepository,
            IDeserializer<SubmitKey> keyDeserializer,
            IDeserializer<string> valueDeserializer,
            ILogger<TopicListener> logger,
            string bootstrapServer)
        {
            _diffGenerator = diffGenerator;
            _diffRepository = diffRepository;
            _logger = logger;

            // TODO: not sure it must be a ConcurrentDictionary here, but keeping it just in case
            var config = new ConcurrentDictionary<string, object>(Config);

            if (!string.IsNullOrWhiteSpace(bootstrapServer))
            {
                config["bootstrap.servers"] = bootstrapServer;
            }

            _logger.Info($"config[bootstrap.servers] = {config["bootstrap.servers"]}.");
            _consumer = consumerFactory.Create(config, keyDeserializer, valueDeserializer);
            _consumer.Assign(new List<TopicPartitionOffset> {new TopicPartitionOffset(Topic, 0, 0)});
        }

        /// <summary>
        /// Read messages. When there is a left/right match, generate diff and save it in repository.
        /// </summary>
        /// <param name="tries">Number of attempts</param>
        /// <exception cref="InvalidOperationException">
        /// If the retrieved message references an unknown side (i.e. must be either 'left' or 'right').
        /// </exception>
        /// <remarks>
        /// Right now cannot be unit-tested, since <see cref="Consumer{TKey,TValue}.Consume(out Confluent.Kafka.Message{TKey,TValue},int)"/> cannot be mocked.
        /// </remarks>
        public void Process(int tries)
        {
            // TODO: do I also need to subscribe?
            for (var i = 0; i < tries; i++)
            {
                if (!_consumer.Consume(out var message, ConsumeTimeoutMS))
                {
                    continue;
                }

                _logger.Info($"Topic: {message.Topic} Partition: {message.Partition} Offset: {message.Offset} {message.Value}");

                // Check if we have read a previous message with same Key.Id
                var cacheRecord = _diffRepository.Load(message.Key.Id) ?? new DiffRecord {Id = message.Key.Id};

                // Set new retrieved side:
                switch (message.Key.Side)
                {
                    case SubmitKey.Left:
                        cacheRecord.Left = message.Value;
                        break;

                    case SubmitKey.Right:
                        cacheRecord.Right = message.Value;
                        break;

                    default:
                        throw new InvalidOperationException($"Unknown side {message.Key.Side}.");
                }

                // Generate new diff:
                try
                {
                    _diffGenerator.RefreshDiff(cacheRecord);
                }
                catch (FormatException e)
                {
                    _logger.Error(e);
                }

                // Save it:
                _diffRepository.Save(cacheRecord);

                _logger.Info($"Message {message.Key} processed.");
            }
        }

        /// <summary>
        /// Disposes the <see cref="_consumer"/> instance.
        /// </summary>
        public void Dispose()
        {
            _consumer?.Dispose();
        }
    }
}
