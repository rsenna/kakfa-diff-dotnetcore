using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kakfka.Diff.Subscriber.Handler.Test;
using Nancy;

namespace Kakfka.Diff.Subscriber.Nancy
{
    public sealed class OutputModule : NancyModule
    {
        private readonly ITestConsumerHandler _testConsumerHandler;

        public OutputModule(
            ITestConsumerHandler testConsumerHandler)
        : base("v1/diff")
        {
            _testConsumerHandler = testConsumerHandler;

            Get("{id:guid", args => ProcessIt(args.id));
        }

        internal async Task<IEnumerable<string>> ProcessIt(Guid id)
        {
            return await _testConsumerHandler.Test(10);
        }
    }
}
