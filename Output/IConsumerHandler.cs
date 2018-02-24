using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kakfka.Diff.Output
{
    public interface IConsumerHandler : IDisposable
    {
        Task<IEnumerable<string>> Test(int take);
    }
}
