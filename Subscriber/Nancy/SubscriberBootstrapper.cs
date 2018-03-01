using Autofac;
using Kafka.Diff.Subscriber.Autofac;
using Nancy;
using Nancy.Bootstrappers.Autofac;
using Nancy.Configuration;

namespace Kafka.Diff.Subscriber.Nancy
{
    public class SubscriberBootstrapper : AutofacNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(ILifetimeScope container)
        {
            container.Update(builder => builder.RegisterModule<SubscriberAutofacModule>());
        }

        public override void Configure(INancyEnvironment environment)
        {
            environment.Tracing(enabled: true, displayErrorTraces: true);
            base.Configure(environment);
        }
    }
}
