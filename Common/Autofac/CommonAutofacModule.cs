using Autofac;
using Kafka.Diff.Common.Impl;

namespace Kafka.Diff.Common.Autofac
{
    public class CommonAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(ConsoleLogger<>)).As(typeof(ILogger<>));
        }
    }
}

