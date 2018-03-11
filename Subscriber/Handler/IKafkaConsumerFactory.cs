using System.Collections.Generic;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

namespace Kafka.Diff.Subscriber.Handler
{
    /// <summary>
    /// Kafka consumer factory.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public interface IKafkaConsumerFactory<TKey, TValue>
    {
        /// <summary>
        /// Creates a new <see cref="Consumer{TKey,TValue}"/> with the specified parameters, using a pre-defined log
        /// configuration.
        /// </summary>
        /// <param name="config">librdkafka configuration parameters.
        /// </param>
        /// <param name="keyDeserializer">
        /// An <see cref="IDeserializer{T}"/> implementation instance for deserializing keys.
        /// </param>
        /// <param name="valueDeserializer">
        /// An <see cref="IDeserializer{T}"/> implementation instance for deserializing values.
        /// </param>
        /// <returns>
        /// A new <see cref="Consumer{TKey,TValue}"/> with the specified parameters & configuration.
        /// </returns>
        /// <remarks>
        /// Refer to <a href="https://github.com/edenhill/librdkafka/blob/master/CONFIGURATION.md">this link</a> for
        /// more info about kafka's configuration options.
        /// </remarks>
        Consumer<TKey, TValue> Create(IDictionary<string, object> config, IDeserializer<TKey> keyDeserializer, IDeserializer<TValue> valueDeserializer);
    }
}
