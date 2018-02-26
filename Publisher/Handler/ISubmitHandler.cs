using System.Threading.Tasks;
using Kafka.Diff.Common;

namespace Kafka.Diff.Publisher.Handler
{
    public interface ISubmitHandler
    {
        Task PostAsync(SubmitKey key, string value);
    }
}
