using System.Linq;
using System.Text;
using Castle.DynamicProxy.Internal;
using FluentAssertions;
using Kafka.Diff.Publisher.Serializer;
using Xunit;

namespace Kafka.Diff.Publisher.Test.Unit.Serializer
{
    public class UTF8SerializerFixture
    {
        private readonly UTF8Serializer _sut = new UTF8Serializer();

        [Fact]
        public void ShouldUseUTF8ForSerialization()
        {
            // Use Castle.DynamicProxy reflection utility to introspect about inner StringSerializer encoding:
            var encodingFieldInfo = typeof(UTF8Serializer)
                .GetAllFields()
                .First(fi => fi.Name == "encoding");
            var encoding = encodingFieldInfo.GetValue(_sut);

            encoding.Should().Be(Encoding.UTF8);
        }
    }
}
