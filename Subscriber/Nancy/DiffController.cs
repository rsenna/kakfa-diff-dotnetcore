using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kakfka.Diff.Subscriber.Handler;
using Kakfka.Diff.Subscriber.Handler.Impl;
using Nancy;

namespace Kakfka.Diff.Subscriber.Nancy
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

            Get("{id:guid", args => GetDiff(args.id));

            // Start worker thread.
            // This is a naive, never-ending implementation of a infinite loop, using tasks
            Task.Run(async () =>
            {
                while (true)
                {
                    // TODO: add CancelationToken support so we can break from the loop
                    _topicListener.Process(10);
                    await Task.Delay(1000);
                }
            });
        }

        public string GetDiff(Guid id)
        {
            var record = _diffRepository.Load(id.ToString());

            if (record == null)
            {
                throw new ArgumentException($"Unknown id {id}.");
            }

            // TODO return json
            return record.ToString();
        }
    }
}
