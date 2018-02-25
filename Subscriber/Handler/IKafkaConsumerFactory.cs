using System.Collections.Generic;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

namespace Kakfka.Diff.Subscriber.Handler
{
    public interface IKafkaConsumerFactory<TKey, TValue>
    {
        Consumer<TKey, TValue> Create(IDictionary<string, object> config, IDeserializer<TKey> keyDeserializer, IDeserializer<TValue> valueDeserializer);
    }
}
