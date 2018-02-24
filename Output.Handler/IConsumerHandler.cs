using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kafka.Diff.Output.Handler
{
    public interface IConsumerHandler : IDisposable
    {
        Task<IEnumerable<string>> Test(int take);
    }
}
