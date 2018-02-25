using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kakfka.Diff.Subscriber.Handler.Test
{
    public interface ITestConsumerHandler : IDisposable
    {
        Task<IEnumerable<string>> Test(int take);
    }
}
