using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kafka.Diff.Subscriber.Handler
{
    public interface ITestConsumerHandler : IDisposable
    {
        Task<IEnumerable<string>> Test(int take);
    }
}
