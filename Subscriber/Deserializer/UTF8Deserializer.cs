using System.Text;
using Confluent.Kafka.Serialization;

namespace Kafka.Diff.Subscriber.Deserializer
{
    /// <summary>
    /// A <see cref="StringDeserializer"/> configured to UTF8 decoding.
    /// </summary>
    public class UTF8Deserializer : StringDeserializer
    {
        public UTF8Deserializer() : base(Encoding.UTF8)
        {}
    }
}
