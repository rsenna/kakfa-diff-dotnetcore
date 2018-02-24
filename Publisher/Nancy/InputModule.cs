using System;
using System.Threading.Tasks;
using Nancy;

namespace Kafka.Diff.Publisher.Nancy
{
    public sealed class InputModule : NancyModule
    {
        private readonly IPublisherHandler _publisherHandler;

        public InputModule(
            IPublisherHandler publisherHandler)
            : base("v1/diff")
        {
            _publisherHandler = publisherHandler;

            Post("{id:guid}/left", args => ProcessIt(args.id));
            Post("{id:guid}/right", args => ProcessIt(args.id));
        }

        internal async Task<string> ProcessIt(Guid id)
        {
            await _publisherHandler.Test(new[] { id.ToString() });
            return "ok";
        }
    }
}
