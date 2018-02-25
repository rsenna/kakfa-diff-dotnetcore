using Autofac;
using Kafka.Diff.Common.Log.Impl;

namespace Kafka.Diff.Common.Log.Autofac
{
    public class SingleInstanceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(ConsoleLogger<>)).As(typeof(ILogger<>));
        }
    }
}

