using Autofac;
using Confluent.Kafka.Serialization;
using Kafka.Diff.Common;
using Kafka.Diff.Common.Autofac;
using Kafka.Diff.Subscriber.Deserializer;
using Kafka.Diff.Subscriber.Handler;
using Kafka.Diff.Subscriber.Handler.Impl;

namespace Kafka.Diff.Subscriber.Autofac
{
    /// <summary>
    /// IoC registration module.
    /// </summary>
    public class SubscriberAutofacModule : Module
    {
        /// <summary>
        /// Used to override default kafka server address.
        /// </summary>
        public static string KafkaServer { get; set; }

        /// <summary>
        /// Adds registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DiffRepository>().As<IDiffRepository>();

            builder.RegisterType<TopicListener>()
                .WithParameter("bootstrapServer", KafkaServer)
                .As<ITopicListener>()
                .SingleInstance();

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
