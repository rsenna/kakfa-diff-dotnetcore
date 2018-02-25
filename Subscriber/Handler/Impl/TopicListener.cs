using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Kafka.Diff.Common.Log;
using Kakfka.Diff.Subscriber.Handler.Impl.Test;

namespace Kakfka.Diff.Subscriber.Handler.Impl
{
    public class TopicListener
    {
        // TODO: inject bootstrap.servers
        public static readonly IDictionary<string, object> ConfigAssign = new ConcurrentDictionary<string, object>
        {
            ["group.id"] = "kafka-diff",
            ["bootstrap.servers"] = "localhost:9092",
            ["enable.auto.commit"] = false
        };

        private readonly IDictionary<string, CacheRecord> _cache;

        // TODO: inject consts through Autofac
        public const string Topic = "diff-topic";
        public const int ConsumeTimeoutMS = 5000;

        private readonly ILogger<TestConsumerAssignHandler> _logger;
        private readonly IDiffGenerator _diffGenerator;
        private readonly IDiffRepository _diffRepository;
        private readonly Consumer<SubmitKey, string> _consumer;

        public TopicListener(
            ILogger<TestConsumerAssignHandler> logger,
            IKafkaConsumerFactory<SubmitKey, string> consumerFactory,
            IDiffGenerator diffGenerator,
            IDiffRepository diffRepository,
            IDeserializer<SubmitKey> keyDeserializer,
            IDeserializer<string> valueDeserializer)
        {
            _logger = logger;
            _diffGenerator = diffGenerator;
            _diffRepository = diffRepository;

            _cache = new ConcurrentDictionary<string, CacheRecord>();

            _consumer = consumerFactory.Create(ConfigAssign, keyDeserializer, valueDeserializer);
            _consumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset(Topic, 0, 0) });

            // TODO: do I also need to subscribe?
        }

        /// <summary>
        /// Read messages. When there is a left/right match, generate diff and save it in repository.
        /// </summary>
        public void Process()
        {
            // TODO: add event so I we can break from the loop
            while (true)
            {
                if (!_consumer.Consume(out var message, ConsumeTimeoutMS))
                {
                    continue;
                }

                // Check if we have read a previous message with same Key.Id
                if (!_cache.TryGetValue(message.Key.Id, out var cacheRecord))
                {
                    cacheRecord = new CacheRecord(message.Key.Id);
                }

                // Set new retrieved side:
                switch (message.Key.Side)
                {
                    case SubmitKey.Left:
                        cacheRecord = cacheRecord.With(newLeft: message.Value);
                        break;

                    case SubmitKey.Right:
                        cacheRecord = cacheRecord.With(newRight: message.Value);
                        break;

                    default:
                        throw new InvalidOperationException($"Unknown side {message.Key.Side}.");
                }

                // Generate new diff:
                cacheRecord = cacheRecord.With(newDiff: _diffGenerator.GetDiff(cacheRecord));

                // Save it:
                _diffRepository.Save(cacheRecord);
            }
        }

        public void Dispose()
        {
            _consumer?.Dispose();
        }
    }
}
