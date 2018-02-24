using Autofac;
using Kakfka.Diff.Output.Handler;
using CommonLogModule=Kafka.Diff.Common.Log.Autofac.SingleInstanceModule;

namespace Kakfka.Diff.Output.Autofac
{
    public class SingleInstanceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<CommonLogModule>();
        }
    }
}
