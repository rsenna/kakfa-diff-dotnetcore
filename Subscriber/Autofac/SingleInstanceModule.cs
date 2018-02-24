using Autofac;
using CommonLogModule=Kafka.Diff.Common.Log.Autofac.SingleInstanceModule;

namespace Kakfka.Diff.Subscriber.Autofac
{
    public class SingleInstanceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<CommonLogModule>();
        }
    }
}
