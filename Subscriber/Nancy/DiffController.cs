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
        private readonly IDiffRepository _diffRepository;

        public DiffController(
            IDiffRepository diffRepository)
        : base("v1/diff")
        {
            _diffRepository = diffRepository;
            Get("{id:guid", args => GetDiff(args.id));
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
