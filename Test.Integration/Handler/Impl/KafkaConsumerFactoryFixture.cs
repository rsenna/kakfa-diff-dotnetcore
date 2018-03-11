using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using Kafka.Diff.Common;
using Kafka.Diff.Subscriber.Deserializer;
using Kafka.Diff.Subscriber.Handler.Impl;
using Xunit;

namespace Kafka.Diff.Test.Integration.Handler.Impl
{
    public class KafkaConsumerFactoryFixture
    {
        private readonly KafkaConsumerFactory<SubmitKey, string> _sut;

        public KafkaConsumerFactoryFixture()
        {
            var aLogger = A.Fake<ILogger<KafkaConsumerFactory<SubmitKey, string>>>();
            _sut = new KafkaConsumerFactory<SubmitKey, string>(aLogger);
        }

        [Fact]
        public void ShouldCreateAConsumerFromAValidConfig_ASubmitKeyDeserializer_AndAnUTF8Deserializer()
        {
            var config = new Dictionary<string, object>
            {
                ["group.id"] = "group-id-is-required-by-kafka"
            };

            var keyDeserializer = new SubmitKeyDeserializer();
            var valueDeserializer = new UTF8Deserializer();

            var result =_sut.Create(config, keyDeserializer, valueDeserializer);

            result.Should().NotBeNull();
        }
    }
}
