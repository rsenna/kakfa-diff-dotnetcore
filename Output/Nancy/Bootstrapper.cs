using Autofac;
using Kakfka.Diff.Output.Autofac;
using Nancy;
using Nancy.Bootstrappers.Autofac;
using SingleInstanceModule = Kafka.Diff.Common.Log.Autofac.SingleInstanceModule;

namespace Kakfka.Diff.Output.Nancy
{
    public class Bootstrapper : AutofacNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(ILifetimeScope container)
        {
            container.Update(builder => builder.RegisterModule<SingleInstanceModule>());
        }

        protected override void ConfigureRequestContainer(ILifetimeScope container, NancyContext context)
        {
            container.Update(builder => builder.RegisterModule<PerRequestModule>());
        }
    }
}
