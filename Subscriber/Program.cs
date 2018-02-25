using System;
using System.Linq;
using Autofac;
using Kafka.Diff.Common;
using Kakfka.Diff.Subscriber.Handler;
using Nancy.Hosting.Self;

namespace Kakfka.Diff.Subscriber
{
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
                var handler = container.Resolve<ITestConsumerHandler>();
                var logger = container.Resolve<ILogger<Program>>();

                var result = handler.Test(10).GetAwaiter().GetResult();
                foreach (var item in result)
                {
                    logger.Info($"Got item: {item}");
                }
            }
        }

        private static void Service()
        {
            // Initialize an instance of NancyHost:
            var configuration = new HostConfiguration {UrlReservations = new UrlReservations {CreateAutomatically = true}};
            var uri = new Uri("http://localhost:12340");

            Console.WriteLine($"Uri: {uri}.");

            var host = new NancyHost(configuration, uri);
            host.Start(); // start hosting

            Console.ReadKey();
            host.Stop(); // stop hosting
        }
    }
}
