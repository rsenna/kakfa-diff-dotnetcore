using Autofac;
using Confluent.Kafka.Serialization;
using Kafka.Diff.Common;
using Kafka.Diff.Common.Autofac;
using Kafka.Diff.Subscriber.Deserializer;
using Kafka.Diff.Subscriber.Handler;
using Kafka.Diff.Subscriber.Handler.Impl;

namespace Kafka.Diff.Subscriber.Autofac
{
    public class SubscriberAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DiffRepository>().As<IDiffRepository>();

            // TODO Only TestConsumerSubscribeHandler will be registered; use an index to choose instead.
            builder.RegisterType<TestConsumerAssignHandler>().As<ITestConsumerHandler>().SingleInstance();
            builder.RegisterType<TestConsumerSubscribeHandler>().As<ITestConsumerHandler>().SingleInstance();

            builder.RegisterType<TopicListener>().As<ITopicListener>().SingleInstance();

            // Helpers:
            builder.RegisterGeneric(typeof(KafkaConsumerFactory<,>)).As(typeof(IKafkaConsumerFactory<,>)).SingleInstance();
            builder.RegisterType<DiffGenerator>().As<IDiffGenerator>().SingleInstance();

            // Deserializers:
            builder.RegisterType<SubmitKeyDeserializer>().As<IDeserializer<SubmitKey>>().SingleInstance();
            builder.RegisterType<UTF8Deserializer>().As<IDeserializer<string>>().SingleInstance();

            // Dependencies:
            builder.RegisterModule<CommonAutofacModule>();
        }
    }
}
