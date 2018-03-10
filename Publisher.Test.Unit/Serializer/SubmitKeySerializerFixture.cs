using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using Kafka.Diff.Publisher.Serializer;
using Xunit;

namespace Kafka.Diff.Publisher.Test.Unit.Serializer
{
    public class SubmitKeySerializerFixture
    {
        private class Config : List<KeyValuePair<string, object>> {}

        private static readonly Config DummyConfig = A.Dummy<Config>();

        private readonly SubmitKeySerializer _sut;

        public SubmitKeySerializerFixture()
        {
            _sut = new SubmitKeySerializer();
        }

        [Fact]
        public void ShouldSerializeUsingUTF8()
        {
            SubmitKeySerializer.InnerSerializer.Should().BeOfType<UTF8Serializer>();
        }

        [Fact]
        public void ShouldNotAcceptANonKeySerialization()
        {
            _sut.Invoking(s => s.Configure(DummyConfig, isKey: false)).Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ShouldCallInnerSerializer()
        {
            _sut.Configure(DummyConfig, isKey: true).Should().BeEquivalentTo(
                SubmitKeySerializer.InnerSerializer.Configure(DummyConfig, isKey: true));
        }
    }
}
