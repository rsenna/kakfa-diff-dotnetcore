using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Kafka.Diff.Common;

namespace Kafka.Diff.Subscriber.Handler.Impl
{
    public class TopicListener : ITopicListener
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

        public TopicListener(
            IKafkaConsumerFactory<SubmitKey, string> consumerFactory,
            IDiffGenerator diffGenerator,
            IDiffRepository diffRepository,
            IDeserializer<SubmitKey> keyDeserializer,
            IDeserializer<string> valueDeserializer)
        {
            _consumerFactory = consumerFactory;
            _diffGenerator = diffGenerator;
            _diffRepository = diffRepository;
            _keyDeserializer = keyDeserializer;
            _valueDeserializer = valueDeserializer;
        }

        /// <summary>
        /// Read messages. When there is a left/right match, generate diff and save it in repository.
        /// </summary>
        public void Process(int tries)
        {
            // TODO: consumer is processing log from the beginning EVERY TIME
            using (var consumer = _consumerFactory.Create(ConfigAssign, _keyDeserializer, _valueDeserializer))
            {
                consumer.Assign(new List<TopicPartitionOffset> {new TopicPartitionOffset(Topic, 0, 0)});

                // TODO: do I also need to subscribe?
                for (var i = 0; i < tries; i++)
                {
                    if (!consumer.Consume(out var message, ConsumeTimeoutMS))
                    {
                        continue;
                    }

                    // Check if we have read a previous message with same Key.Id
                    var cacheRecord = _diffRepository.Load(message.Key.Id) ?? new CacheRecord {Id = message.Key.Id};

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
                    _diffGenerator.RefreshDiff(cacheRecord);

                    // Save it:
                    _diffRepository.Save(cacheRecord);
                }
            }
        }
    }
}
