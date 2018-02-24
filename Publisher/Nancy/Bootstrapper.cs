using Autofac;
using Kafka.Diff.Publisher.Autofac;
using Nancy.Bootstrappers.Autofac;

namespace Kafka.Diff.Publisher.Nancy
{
    public class Bootstrapper : AutofacNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(ILifetimeScope container)
        {
            container.Update(builder => builder.RegisterModule<SingleInstanceModule>());
        }
    }
}
