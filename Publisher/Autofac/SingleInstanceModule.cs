using Autofac;
using Confluent.Kafka.Serialization;
using Kafka.Diff.Common.Log;
using Kafka.Diff.Publisher.Handler;
using Kafka.Diff.Publisher.Handler.Test;
using Kafka.Diff.Publisher.Handler.Test.Impl;
using Kafka.Diff.Publisher.Serializer;
using CommonLogModule=Kafka.Diff.Common.Log.Autofac.SingleInstanceModule;

namespace Kafka.Diff.Publisher.Autofac
{
    public class SingleInstanceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Serializers:
            builder.RegisterType<SubmitKeySerializer>().As<ISerializer<SubmitKey>>().SingleInstance();
            builder.RegisterType<UTF8Serializer>().As<ISerializer<string>>().SingleInstance();

            // Handlers:
            builder.RegisterType<TestProducerHandler>().As<ITestProducerHandler>().SingleInstance();

            // Dependencies:
            builder.RegisterModule<CommonLogModule>();
        }
    }
}
