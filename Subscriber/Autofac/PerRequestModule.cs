using Autofac;
using Kakfka.Diff.Subscriber.Handler;

namespace Kakfka.Diff.Subscriber.Autofac
{
    public class PerRequestModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<KafkaSubscriberHandler>().As<ISubscriberHandler>();
        }
    }
}
