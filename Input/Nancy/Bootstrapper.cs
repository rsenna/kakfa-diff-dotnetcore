using Autofac;
using Kafka.Diff.Input.Autofac;
using Nancy.Bootstrappers.Autofac;

namespace Kafka.Diff.Input.Nancy
{
    public class Bootstrapper : AutofacNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(ILifetimeScope container)
        {
            container.Update(builder => builder.RegisterModule<SingleInstanceModule>());
        }
    }
}
