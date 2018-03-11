using Autofac;
using Kafka.Diff.Subscriber.Autofac;
using Nancy;
using Nancy.Bootstrappers.Autofac;
using Nancy.Configuration;

namespace Kafka.Diff.Subscriber.Nancy
{
    /// <summary>
    /// Bootstrapper IoC class needed by Nancy
    /// </summary>
    public class SubscriberBootstrapper : AutofacNancyBootstrapper
    {
        /// <summary>
        /// Configure the application level container with any additional registrations.
        /// Makes Nancy use our Autofac container (instead of embedded TinyIOC)
        /// </summary>
        /// <param name="container">An Autofac's <see cref="ILifetimeScope"/> scope instance.</param>
        protected override void ConfigureApplicationContainer(ILifetimeScope container)
        {
            container.Update(builder => builder.RegisterModule<SubscriberAutofacModule>());
        }

        /// <summary>
        /// Configures the Nancy environment.
        /// </summary>
        /// <param name="environment">The instance to configure.</param>
        public override void Configure(INancyEnvironment environment)
        {
            environment.Tracing(enabled: true, displayErrorTraces: true);
            base.Configure(environment);
        }
    }
}
