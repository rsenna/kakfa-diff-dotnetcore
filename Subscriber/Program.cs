using System;
using System.Linq;
using Autofac;
using Kafka.Diff.Common.Log;
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
                var handler = container.Resolve<ISubscriberHandler>();
                var logger = container.Resolve<ILogger<Program>>();

                var result = handler.Test(10).GetAwaiter().GetResult();
                foreach (var item in result)
                {
                    logger.Info($"Got item: ${item}");
                }
            }
        }

        private static void Service()
        {
            // initialize an instance of NancyHost (found in the Nancy.Hosting.Self package)
            var host = new NancyHost(new Uri("http://localhost:23456"));
            host.Start(); // start hosting

            Console.ReadKey();
            host.Stop(); // stop hosting
        }
    }
}
