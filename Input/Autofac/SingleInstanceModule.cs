using Autofac;
using Kafka.Diff.Input.Handler;
using CommonLogModule=Kafka.Diff.Common.Log.Autofac.SingleInstanceModule;

namespace Kafka.Diff.Input.Autofac
{
    public class SingleInstanceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<KafkaProducerHandler>().As<IProducerHandler>().SingleInstance();
            builder.RegisterModule<CommonLogModule>();
        }
    }
}
