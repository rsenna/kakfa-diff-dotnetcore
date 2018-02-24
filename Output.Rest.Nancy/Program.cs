using System;
using Nancy.Hosting.Self;

namespace Output.Rest.Nancy
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // initialize an instance of NancyHost (found in the Nancy.Hosting.Self package)
            var host = new NancyHost(new Uri("http://localhost:23456"));
            host.Start(); // start hosting

            Console.ReadKey();
            host.Stop(); // stop hosting
        }
    }
}
