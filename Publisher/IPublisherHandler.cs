using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kafka.Diff.Publisher
{
    public interface IPublisherHandler
    {
        Task Test(ICollection<string> items);
    }
}
