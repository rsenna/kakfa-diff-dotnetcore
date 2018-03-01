using Autofac;
using Kafka.Diff.Publisher.Autofac;
using Nancy;
using Nancy.Bootstrappers.Autofac;
using Nancy.Configuration;

namespace Kafka.Diff.Publisher.Nancy
{
    public class PublisherBootstrapper : AutofacNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(ILifetimeScope container)
        {
            container.Update(builder => builder.RegisterModule<PublisherAutofacModule>());
        }

        public override void Configure(INancyEnvironment environment)
        {
            environment.Tracing(enabled: true, displayErrorTraces: true);
            base.Configure(environment);
        }
    }
}
