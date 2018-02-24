using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kakfka.Diff.Subscriber
{
    public interface ISubscriberHandler : IDisposable
    {
        Task<IEnumerable<string>> Test(int take);
    }
}
