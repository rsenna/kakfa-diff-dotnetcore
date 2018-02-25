using Autofac;
using Kakfka.Diff.Subscriber.Handler;

namespace Kakfka.Diff.Subscriber.Autofac
{
    public class PerRequestModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(KafkaConsumerFactory<,>)).As(typeof(IKafkaConsumerFactory<,>));

//            builder.RegisterType<KafkaAssignHandler>().As<ISubscriberHandler>();
            builder.RegisterType<KafkaSubscribeHandler>().As<ISubscriberHandler>();
        }
    }
}
