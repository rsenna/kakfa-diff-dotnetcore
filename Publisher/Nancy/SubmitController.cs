using System;
using System.Threading.Tasks;
using Kafka.Diff.Common;
using Kafka.Diff.Publisher.Handler;
using Nancy;
using Nancy.ModelBinding;

namespace Kafka.Diff.Publisher.Nancy
{
    public sealed class SubmitController : NancyModule
    {
        private readonly ISubmitHandler _submitHandler;

        public SubmitController(ISubmitHandler submitHandler)
            : base("v1/diff")
        {
            _submitHandler = submitHandler;

            Post("{id:guid}/left", async (args, ct) => await PostIt(args.id, SubmitKey.Left));
            Post("{id:guid}/right", async (args, ct) => await PostIt(args.id, SubmitKey.Right));
        }

        public async Task<Response> PostIt(Guid id, string side)
        {
            try
            {
                var submitKey = new SubmitKey(id, side);
                var debug = this.Request.Body.AsString();
                var request = this.Bind<Request>();
                await _submitHandler.PostAsync(submitKey, request.Data);

                return Response.AsJson(new {Message = "OK"});
            }
            catch (Exception ex)
            {
                return Response.AsJson(new {ex.Message, ex.Source, ex.StackTrace}, HttpStatusCode.InternalServerError);
            }
        }
    }
}
