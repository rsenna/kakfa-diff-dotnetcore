using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nancy;

namespace Kakfka.Diff.Subscriber.Nancy
{
    public sealed class OutputModule : NancyModule
    {
        private readonly ISubscriberHandler _subscriberHandler;

        public OutputModule(
            ISubscriberHandler subscriberHandler)
        : base("v1/diff")
        {
            _subscriberHandler = subscriberHandler;

            Get("{id:guid", args => ProcessIt(args.id));
        }

        internal async Task<IEnumerable<string>> ProcessIt(Guid id)
        {
            return await _subscriberHandler.Test(10);
        }
    }
}
