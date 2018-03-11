using System.Threading.Tasks;
using Kafka.Diff.Common;

namespace Kafka.Diff.Publisher.Handler
{
    /// <summary>
    /// Main service for submitting diffs (left and right sides).
    /// </summary>
    public interface ISubmitHandler
    {
        /// <summary>
        /// Submits key-value pair to kafka topic.
        /// </summary>
        /// <param name="key">A <see cref="SubmitKey"/> instance.</param>
        /// <param name="value">A <see cref="string"/> instance.</param>
        /// <returns>
        /// A <see cref="Task"/> representing a single asynchronous operation that does not return a value.
        /// </returns>
        Task PostAsync(SubmitKey key, string value);
    }
}
