using Autofac;

namespace Kafka.Diff.Common.Log.Autofac
{
    public class SingleInstanceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(ILogger<>)).As(typeof(ILogger<>)).SingleInstance();
        }
    }
}
