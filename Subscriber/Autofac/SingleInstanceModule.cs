using Autofac;
using Confluent.Kafka.Serialization;
using Kafka.Diff.Common.Log;
using Kakfka.Diff.Subscriber.Deserializer;
using Kakfka.Diff.Subscriber.Handler;
using Kakfka.Diff.Subscriber.Handler.Impl;
using CommonLogModule=Kafka.Diff.Common.Log.Autofac.SingleInstanceModule;

namespace Kakfka.Diff.Subscriber.Autofac
{
    public class SingleInstanceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Helpers:
            builder.RegisterType<DiffGenerator>().As<IDiffGenerator>().SingleInstance();

            // Deserializers:
            builder.RegisterType<SubmitKeyDeserializer>().As<IDeserializer<SubmitKey>>().SingleInstance();
            builder.RegisterType<UTF8Deserializer>().As<IDeserializer<string>>().SingleInstance();

            // Dependencies:
            builder.RegisterModule<CommonLogModule>();
        }
    }
}
