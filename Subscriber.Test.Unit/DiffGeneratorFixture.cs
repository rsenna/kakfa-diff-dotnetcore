using System;
using FluentAssertions;
using Kafka.Diff.Subscriber.Handler;
using Kafka.Diff.Subscriber.Handler.Impl;
using Xunit;

namespace Kafka.Diff.Subscriber.Test.Unit
{
    public class DiffGeneratorFixture
    {
        private readonly DiffGenerator _sut;

        public DiffGeneratorFixture()
        {
            _sut = new DiffGenerator();
        }

        [Fact]
        public void ShouldIgnoreIncompleteRecord()
        {
            var record = new DiffRecord {Left = "something", Right = null};

            _sut.RefreshDiff(record);

            record.Analysis.Should().BeNull();
        }

        [Fact]
        public void ShouldNotAcceptInvalidBase64()
        {
            var record = new DiffRecord {Left = "not-valid", Right = "not-valid"};

            _sut.Invoking(s => s.RefreshDiff(record)).Should().Throw<FormatException>();
        }

        [Fact]
        public void ShouldTellIfContentsAreEqual()
        {
            var array = new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
            var base64 = Convert.ToBase64String(array);
            var record = new DiffRecord {Left = base64, Right = base64};

            _sut.RefreshDiff(record);

            record.Analysis.Should().NotBeNull();
            record.Analysis.EqualSize.Should().BeTrue();
            record.Analysis.EqualContent.Should().BeTrue();
        }

        [Fact]
        public void ShouldTellIfContentsAreDifferent()
        {
            var leftArray  = new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12}; // AQIDBAUGBwgJCgsM
            var rightArray = new byte[] {1, 2, 3, 7, 6, 5, 4, 8, 9, 10, 11, 13}; // AQIDBwYFBAgJCgsN

            var left = Convert.ToBase64String(leftArray);
            var right = Convert.ToBase64String(rightArray);

            var record = new DiffRecord {Left = left, Right = right};

            _sut.RefreshDiff(record);

            record.Analysis.Should().NotBeNull();
            record.Analysis.EqualSize.Should().BeTrue();
            record.Analysis.EqualContent.Should().BeFalse();

            record.Analysis.Offsets.Count.Should().Be(4);
            record.Analysis.Offsets.Should().BeEquivalentTo(
                new DiffRecord.DiffOffset {IsEqual = true, Length = 3},
                new DiffRecord.DiffOffset {IsEqual = false, Length = 4},
                new DiffRecord.DiffOffset {IsEqual = true, Length = 4},
                new DiffRecord.DiffOffset {IsEqual = false, Length = 1});
        }
    }
}
