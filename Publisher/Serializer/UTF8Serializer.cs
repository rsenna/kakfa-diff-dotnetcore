using System.Text;
using Confluent.Kafka.Serialization;

namespace Kafka.Diff.Publisher.Serializer
{
    public class UTF8Serializer : StringSerializer
    {
        public UTF8Serializer() : base(Encoding.UTF8)
        {}
    }
}
