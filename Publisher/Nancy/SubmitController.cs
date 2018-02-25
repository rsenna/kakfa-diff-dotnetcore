using System;
using System.Threading.Tasks;
using Kafka.Diff.Common.Log;
using Kafka.Diff.Publisher.Handler;
using Nancy;

namespace Kafka.Diff.Publisher.Nancy
{
    public sealed class SubmitController : NancyModule
    {
        private readonly ISubmitHandler _submitHandler;

        public SubmitController(
            ISubmitHandler submitHandler)
            : base("v1/diff")
        {
            _submitHandler = submitHandler;

            Post("{id:guid}/left", args => PostIt(args.id, SubmitKey.Left));
            Post("{id:guid}/right", args => PostIt(args.id, SubmitKey.Right));
        }

        public async Task<string> PostIt(Guid id, string side)
        {
            if (id == null)
            {
                throw new ArgumentException("id cannot be null.", nameof(id));
            }

            // TODO make id be a guid in the whole application
            var submitKey = new SubmitKey(id.ToString(), side);
            var value = Request.Body.AsString();
            await _submitHandler.Post(submitKey, value);

            // TODO implement proper result
            return "ok";
        }
    }
}
