using System;
using System.Threading.Tasks;
using Nancy;

namespace Kafka.Diff.Input.Nancy
{
    public sealed class InputModule : NancyModule
    {
        private readonly IProducerHandler _producerHandler;

        public InputModule(
            IProducerHandler producerHandler)
            : base("v1/diff")
        {
            _producerHandler = producerHandler;

            Post("{id:guid}/left", args => ProcessIt(args.id));
            Post("{id:guid}/right", args => ProcessIt(args.id));
        }

        internal async Task<string> ProcessIt(Guid id)
        {
            await _producerHandler.Test(new[] { id.ToString() });
            return "ok";
        }
    }
}
