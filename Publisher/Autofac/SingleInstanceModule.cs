using Autofac;
using Kafka.Diff.Publisher.Handler;
using Kafka.Diff.Publisher.Handler.Test;
using Kafka.Diff.Publisher.Handler.Test.Impl;
using CommonLogModule=Kafka.Diff.Common.Log.Autofac.SingleInstanceModule;

namespace Kafka.Diff.Publisher.Autofac
{
    public class SingleInstanceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TestProducerHandler>().As<ITestProducerHandler>().SingleInstance();
            builder.RegisterModule<CommonLogModule>();
        }
    }
}
