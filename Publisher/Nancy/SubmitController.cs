using System;
using System.Threading.Tasks;
using Kafka.Diff.Common;
using Kafka.Diff.Publisher.Handler;
using Nancy;
using Nancy.ModelBinding;

namespace Kafka.Diff.Publisher.Nancy
{
    /// <summary>
    /// Main controller API class for the Publisher module
    /// </summary>
    public sealed class SubmitController : NancyModule
    {
        public class SubmitResponse
        {
            public object Body { get; set; }
            public HttpStatusCode StatusCode { get; set; }
        }

        private readonly ISubmitHandler _submitHandler;

        public SubmitController(ISubmitHandler submitHandler)
            : base("v1/diff")
        {
            _submitHandler = submitHandler;

            Post("{id:guid}/left", async (args, ct) => await PostIt(args.id, SubmitKey.Left));
            Post("{id:guid}/right", async (args, ct) => await PostIt(args.id, SubmitKey.Right));
        }

        private async Task<Response> PostIt(Guid id, string side)
        {
            var request = this.Bind<PublisherRequest>();
            var response = await Post(id, side, request);
            return Response.AsJson(response.Body, response.StatusCode);
        }

        public async Task<SubmitResponse> Post(Guid id, string side, PublisherRequest request)
        {
            try
            {
                var submitKey = new SubmitKey(id, side);
                await _submitHandler.PostAsync(submitKey, request.Data);

                return new SubmitResponse
                {
                    Body = new {Message = "OK"},
                    StatusCode = HttpStatusCode.Accepted
                };
            }
            catch (Exception ex)
            {
                return new SubmitResponse
                {
                    Body = new {ex.Message, ex.Source, ex.StackTrace},
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
