using System;
using System.Threading.Tasks;
using Kafka.Diff.Subscriber.Handler;
using Nancy;

namespace Kafka.Diff.Subscriber.Nancy
{
    public sealed class DiffController : NancyModule
    {
        public class DiffResponse
        {
            public object Body { get; set; }
            public HttpStatusCode StatusCode { get; set; }
        }

        private readonly ITopicListener _topicListener;
        private readonly IDiffRepository _diffRepository;

        public DiffController(
            ITopicListener topicListener,
            IDiffRepository diffRepository)
        : base("v1/diff")
        {
            _topicListener = topicListener;
            _diffRepository = diffRepository;

            Get("{id}", args =>
            {
                Guid id = args.id;
                return Response.AsJson(GetDiff(id).Body, GetDiff(id).StatusCode);
            });

            StartWorker();
        }

        /// <summary>
        /// Start worker thread.
        /// </summary>
        /// <remarks>
        /// This is a naive, never-ending implementation of a infinite loop, using tasks.
        /// </remarks>
        public void StartWorker()
        {
            Task.Run(async () =>
            {
                // TODO: add CancelationToken support so we can break from the loop
                while (true)
                {
                    _topicListener.Process(10);
                    await Task.Delay(1000);
                }
            });
        }

        public DiffResponse GetDiff(Guid id)
        {
            try
            {
                var record = _diffRepository.Load(id);

                return new DiffResponse
                {
                    Body = record == null
                        ? new {Message = $"Unknown id {id}."}
                        : (object) record,
                    StatusCode = HttpStatusCode.NotFound
                };
            }
            catch (Exception ex)
            {
                return new DiffResponse
                {
                    Body = new {ex.Message, ex.Source, ex.StackTrace},
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
