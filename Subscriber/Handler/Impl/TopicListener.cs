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
        // TODO: inject bootstrap.servers
        public static readonly IDictionary<string, object> ConfigAssign = new ConcurrentDictionary<string, object>
        {
            ["group.id"] = "kafka-diff",
            ["bootstrap.servers"] = "localhost:9092",
            ["enable.auto.commit"] = false
        };

        // TODO: inject consts through Autofac
        public const string Topic = "diff-topic";
        public const int ConsumeTimeoutMS = 5000;

        private readonly IKafkaConsumerFactory<SubmitKey, string> _consumerFactory;
        private readonly IDiffGenerator _diffGenerator;
        private readonly IDiffRepository _diffRepository;
        private readonly IDeserializer<SubmitKey> _keyDeserializer;
        private readonly IDeserializer<string> _valueDeserializer;
        private readonly ILogger<TopicListener> _logger;
        private Consumer<SubmitKey, string> _consumer;

        public TopicListener(
            IKafkaConsumerFactory<SubmitKey, string> consumerFactory,
            IDiffGenerator diffGenerator,
            IDiffRepository diffRepository,
            IDeserializer<SubmitKey> keyDeserializer,
            IDeserializer<string> valueDeserializer,
            ILogger<TopicListener> logger)
        {
            _consumerFactory = consumerFactory;
            _diffGenerator = diffGenerator;
            _diffRepository = diffRepository;
            _keyDeserializer = keyDeserializer;
            _valueDeserializer = valueDeserializer;
            _logger = logger;

            _consumer = _consumerFactory.Create(ConfigAssign, _keyDeserializer, _valueDeserializer);
            _consumer.Assign(new List<TopicPartitionOffset> {new TopicPartitionOffset(Topic, 0, 0)});
        }

        /// <summary>
        /// Read messages. When there is a left/right match, generate diff and save it in repository.
        /// </summary>
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

        public void Dispose()
        {
            _consumer?.Dispose();
        }
    }
}
