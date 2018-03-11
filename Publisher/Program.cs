using System;
using System.Linq;
using Autofac;
using Kafka.Diff.Publisher.Autofac;
using Kafka.Diff.Publisher.Handler;
using Nancy.Hosting.Self;

namespace Kafka.Diff.Publisher
{
    /// <summary>
    /// Initializes the Publisher application.
    /// </summary>
    internal class Program
    {
        public const string HostedUrl = "http://localhost:12345";

        /// <summary>
        /// Entry point of the <see cref="Kafka.Diff.Publisher"/> application.
        /// Initializes the self-host Nancy environment on the specified url.
        /// </summary>
        private static void Main(string[] args)
        {
            if (args.Contains("--gitlab"))
            {
                PublisherAutofacModule.KafkaServer = "spotify-kafka:9092";
            }

            // Initialize an instance of NancyHost:
            var configuration = new HostConfiguration {UrlReservations = new UrlReservations {CreateAutomatically = true}};
            var uri = new Uri(HostedUrl);

            Console.WriteLine($"Uri: {uri}.");

            var host = new NancyHost(configuration, uri);
            host.Start(); // start hosting

            Console.ReadKey();
            host.Stop(); // stop hosting
        }
    }
}
