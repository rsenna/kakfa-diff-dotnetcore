using System;
using System.Threading.Tasks;
using Kafka.Diff.Publisher.Handler.Test;
using Nancy;

namespace Kafka.Diff.Publisher.Nancy
{
    public sealed class InputModule : NancyModule
    {
        private readonly ITestProducerHandler _testProducerHandler;

        public InputModule(
            ITestProducerHandler testProducerHandler)
            : base("v1/diff")
        {
            _testProducerHandler = testProducerHandler;

            Post("{id:guid}/left", args => ProcessIt(args.id));
            Post("{id:guid}/right", args => ProcessIt(args.id));
        }

        internal async Task<string> ProcessIt(Guid id)
        {
            await _testProducerHandler.Test(new[] { id.ToString() });
            return "ok";
        }
    }
}
