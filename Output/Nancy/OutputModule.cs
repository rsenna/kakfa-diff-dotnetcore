using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nancy;

namespace Kakfka.Diff.Output.Nancy
{
    public sealed class OutputModule : NancyModule
    {
        private readonly IConsumerHandler _consumerHandler;

        public OutputModule(
            IConsumerHandler consumerHandler)
        : base("v1/diff")
        {
            _consumerHandler = consumerHandler;

            Get("{id:guid", args => ProcessIt(args.id));
        }

        internal async Task<IEnumerable<string>> ProcessIt(Guid id)
        {
            return await _consumerHandler.Test(10);
        }
    }
}
