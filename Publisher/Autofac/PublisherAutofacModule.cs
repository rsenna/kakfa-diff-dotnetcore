using Autofac;
using Autofac.Core;
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
            // Serializers:
            builder.RegisterType<SubmitKeySerializer>().As<ISerializer<SubmitKey>>().SingleInstance();
            builder.RegisterType<UTF8Serializer>().As<ISerializer<string>>().SingleInstance();

            // Handlers:
            builder.RegisterType<SubmitHandler>()
                .WithParameter("bootstrapServer", KafkaServer)
                .As<ISubmitHandler>()
                .SingleInstance();

            // Dependencies:
            builder.RegisterModule<CommonAutofacModule>();
        }
    }
}
