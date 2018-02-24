using Autofac;
using Kakfa.Diff.Input.Handler;
using CommonLogModule=Kafka.Diff.Common.Log.Autofac.SingleInstanceModule;

namespace Kakfa.Diff.Input.Autofac
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
