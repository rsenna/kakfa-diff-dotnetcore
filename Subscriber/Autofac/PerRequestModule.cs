using Autofac;
using Kakfka.Diff.Subscriber.Handler;
using Kakfka.Diff.Subscriber.Handler.Impl;
using Kakfka.Diff.Subscriber.Handler.Impl.Test;
using Kakfka.Diff.Subscriber.Handler.Test;

namespace Kakfka.Diff.Subscriber.Autofac
{
    public class PerRequestModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(KafkaConsumerFactory<,>)).As(typeof(IKafkaConsumerFactory<,>));

            builder.RegisterType<DiffRepository>().As<IDiffRepository>();

            // TODO Only TestConsumerSubscribeHandler will be registered; use an index to choose instead.
            builder.RegisterType<TestConsumerAssignHandler>().As<ITestConsumerHandler>();
            builder.RegisterType<TestConsumerSubscribeHandler>().As<ITestConsumerHandler>();
        }
    }
}
