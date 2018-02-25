using System.Threading.Tasks;
using Kafka.Diff.Common;

namespace Kafka.Diff.Publisher.Handler
{
    public interface ISubmitHandler
    {
        Task Post(SubmitKey key, string value);
    }
}
