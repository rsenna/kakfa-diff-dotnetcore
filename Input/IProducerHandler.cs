using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kakfa.Diff.Input
{
    public interface IProducerHandler
    {
        Task Test(ICollection<string> items);
    }
}
