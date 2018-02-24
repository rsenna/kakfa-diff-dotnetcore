using Autofac;
using Kakfa.Diff.Input.Autofac;
using Nancy.Bootstrappers.Autofac;

namespace Kakfa.Diff.Input.Nancy
{
    public class Bootstrapper : AutofacNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(ILifetimeScope container)
        {
            container.Update(builder => builder.RegisterModule<SingleInstanceModule>());
        }
    }
}
