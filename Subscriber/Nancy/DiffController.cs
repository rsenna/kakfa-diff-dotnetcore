using System;
using System.Threading.Tasks;
using Kafka.Diff.Subscriber.Handler;
using Nancy;

namespace Kafka.Diff.Subscriber.Nancy
{
    public sealed class DiffController : NancyModule
    {
        private readonly ITopicListener _topicListener;
        private readonly IDiffRepository _diffRepository;

        public DiffController(
            ITopicListener topicListener,
            IDiffRepository diffRepository)
        : base("v1/diff")
        {
            _topicListener = topicListener;
            _diffRepository = diffRepository;

            Get("{id}", args => GetDiff(args.id));

            StartWorker();
        }

        /// <summary>
        /// Start worker thread.
        /// </summary>
        /// <remarks>
        /// This is a naive, never-ending implementation of a infinite loop, using tasks.
        /// </remarks>
        private void StartWorker()
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

        public Response GetDiff(Guid id)
        {
            try
            {
                var record = _diffRepository.Load(id);

                return record == null
                    ? Response.AsJson(new {Message = $"Unknown id {id}."}, HttpStatusCode.NotFound)
                    : Response.AsJson(record);
            }
            catch (Exception ex)
            {
                return Response.AsJson(new {ex.Message, ex.Source, ex.StackTrace}, HttpStatusCode.InternalServerError);
            }
        }
    }
}
