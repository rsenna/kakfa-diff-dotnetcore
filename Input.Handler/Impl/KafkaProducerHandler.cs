using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

namespace Kafka.Diff.Input.Handler.Impl
{
    public class KafkaProducerHandler : IProducerHandler
    {
        public static readonly IDictionary<string, object> Config = new ConcurrentDictionary<string, object>
        {
            ["bootstrap.servers"] = "localhost:9092"
        };

        public static readonly StringSerializer DefaultSerializer = new StringSerializer(Encoding.UTF8);

        public async Task Test(ICollection<string> items)
        {
            using (var producer = new Producer<Null, string>(Config, null, DefaultSerializer))
            {
                foreach (var it in items)
                {
                    await producer.ProduceAsync("hello-topic", null, it);
                }

                producer.Flush(100);
            }
        }
    }
}
