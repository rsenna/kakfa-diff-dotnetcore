using Autofac;
using Confluent.Kafka.Serialization;
using Kafka.Diff.Common;
using Kafka.Diff.Common.Autofac;
using Kafka.Diff.Publisher.Handler;
using Kafka.Diff.Publisher.Handler.Impl;
using Kafka.Diff.Publisher.Serializer;

namespace Kafka.Diff.Publisher.Autofac
{
    /// <summary>
    /// IoC registration module.
    /// </summary>
    public class PublisherAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Serializers:
            builder.RegisterType<SubmitKeySerializer>().As<ISerializer<SubmitKey>>().SingleInstance();
            builder.RegisterType<UTF8Serializer>().As<ISerializer<string>>().SingleInstance();

            // Handlers:
            builder.RegisterType<TestProducerHandler>().As<ITestProducerHandler>().SingleInstance();
            builder.RegisterType<SubmitHandler>().As<ISubmitHandler>().SingleInstance();

            // Dependencies:
            builder.RegisterModule<CommonAutofacModule>();
        }
    }
}
