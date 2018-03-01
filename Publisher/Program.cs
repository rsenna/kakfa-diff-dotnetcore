using System;
using System.Linq;
using Autofac;
using Kafka.Diff.Publisher.Handler;
using Nancy.Hosting.Self;

namespace Kafka.Diff.Publisher
{
    /// <summary>
    /// Initializes the Publisher application.
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Contains("--test"))
            {
                Test();
            }
            else if (args.Contains("--service"))
            {
                Service();
            }
        }

        private static void Test()
        {
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyModules(typeof(Program).Assembly);

            using (var container = builder.Build())
            {
                var handler = container.Resolve<ITestProducerHandler>();
                handler.Test(new [] {"this", "is", "a", "test", "b", "c", "d", "e", "f", "g" }).GetAwaiter().GetResult();
            }
        }

        private static void Service()
        {
            // Initialize an instance of NancyHost:
            var configuration = new HostConfiguration {UrlReservations = new UrlReservations {CreateAutomatically = true}};
            var uri = new Uri("http://localhost:12345");

            Console.WriteLine($"Uri: {uri}.");

            var host = new NancyHost(configuration, uri);
            host.Start(); // start hosting

            Console.ReadKey();
            host.Stop(); // stop hosting
        }
    }
}
