using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kafka.Diff.Input.Handler
{
    public interface IProducerHandler
    {
        Task Test(ICollection<string> items);
    }
}
