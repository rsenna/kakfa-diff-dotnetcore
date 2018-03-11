using System;

namespace Kafka.Diff.Subscriber.Handler
{
    /// <summary>
    /// Listener used to process messages in the `diff-topic` queued.
    /// </summary>
    public interface ITopicListener
    {
        /// <summary>
        /// Read messages. When there is a left/right match, generate diff and save it in repository.
        /// </summary>
        /// <param name="tries">Number of attempts</param>
        /// <exception cref="InvalidOperationException">
        /// If the retrieved message references an unknown side (i.e. must be either 'left' or 'right').
        /// </exception>
        void Process(int tries);
    }
}
