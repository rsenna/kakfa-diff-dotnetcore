using System;
using FluentAssertions;
using Kafka.Diff.Subscriber.Handler;
using Kafka.Diff.Subscriber.Handler.Impl;
using Xunit;

namespace Kafka.Diff.Test.Integration.Handler.Impl
{
    public class SharedState
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DiffRepository Sut { get; } = new DiffRepository();
    }

    [TestCaseOrderer(
        "Kafka.Diff.Test.Integration.AlphabeticalOrderer",
        "Kafka.Diff.Test.Integration")]
    public class DiffRepositoryFixture : IClassFixture<SharedState>
    {
        private readonly Guid _id;
        private readonly DiffRepository _sut;

        public DiffRepositoryFixture(SharedState sharedState)
        {
            _id = sharedState.Id;
            _sut = sharedState.Sut;
        }

        [Fact]
        public void A_ShouldSaveARecord()
        {
            var record = new DiffRecord
            {
                Id = _id,
                Left = "left",
                Right = "right"
            };

            var result = _sut.Save(record);

            result.Should().BeTrue();
        }

        [Fact]
        public void B_ShouldLoadSavedRecord()
        {
            var record = _sut.Load(_id);

            record.Should().NotBeNull();
            record.Id.Should().Be(_id);
            record.Left.Should().Be("left");
            record.Right.Should().Be("right");
            record.Analysis.Should().BeNull();
        }

        [Fact]
        public void ShouldNotLoadNonExistentRecord()
        {
            var record = _sut.Load(Guid.NewGuid());

            record.Should().BeNull();
        }

        [Fact]
        public void C_ShouldDeleteSavedRecord()
        {
            var result = _sut.Delete(_id);

            result.Should().Be(true);
        }

        [Fact]
        public void ShouldNotDeleteNonExistentRecord()
        {
            var result = _sut.Delete(Guid.NewGuid());

            result.Should().Be(false);
        }
    }
}
