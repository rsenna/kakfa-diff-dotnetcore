﻿using System.Text;
using Confluent.Kafka.Serialization;

namespace Kakfka.Diff.Subscriber.Deserializer
{
    public class UTF8Deserializer : StringDeserializer
    {
        public UTF8Deserializer() : base(Encoding.UTF8)
        {}
    }
}
