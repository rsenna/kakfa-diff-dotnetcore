using Autofac;
using Kakfka.Diff.Output.Handler;

namespace Kakfka.Diff.Output.Autofac
{
    public class PerRequestModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<KafkaConsumerHandler>().As<IConsumerHandler>().InstancePerRequest();
        }
    }
}
