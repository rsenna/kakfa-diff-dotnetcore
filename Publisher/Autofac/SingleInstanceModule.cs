using Autofac;
using Kafka.Diff.Publisher.Handler;
using CommonLogModule=Kafka.Diff.Common.Log.Autofac.SingleInstanceModule;

namespace Kafka.Diff.Publisher.Autofac
{
    public class SingleInstanceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<KafkaPublisherHandler>().As<IPublisherHandler>().SingleInstance();
            builder.RegisterModule<CommonLogModule>();
        }
    }
}
