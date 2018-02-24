using System;
using System.Linq;
using Autofac;
using Nancy.Hosting.Self;

namespace Kafka.Diff.Input
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

            builder.RegisterAssemblyModules();

            using (var container = builder.Build())
            {
                var handler = container.Resolve<IProducerHandler>();
                handler.Test(new [] {"this", "is", "a", "test", "b", "c", "d", "e", "f", "g" }).GetAwaiter().GetResult();
            }
        }

        private static void Service()
        {
            // initialize an instance of NancyHost (found in the Nancy.Hosting.Self package)
            var host = new NancyHost(new Uri("http://localhost:12345"));
            host.Start(); // start hosting

            Console.ReadKey();
            host.Stop(); // stop hosting
        }
    }
}
