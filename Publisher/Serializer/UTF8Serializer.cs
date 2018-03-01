using System.Text;
using Confluent.Kafka.Serialization;

namespace Kafka.Diff.Publisher.Serializer
{
    /// <summary>
    /// A <see cref="StringSerializer"/> configured to UTF8 encoding.
    /// </summary>
    public class UTF8Serializer : StringSerializer
    {
        public UTF8Serializer() : base(Encoding.UTF8)
        {}
    }
}
