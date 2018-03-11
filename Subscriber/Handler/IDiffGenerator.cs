using Kafka.Diff.Subscriber.Handler.Impl;

namespace Kafka.Diff.Subscriber.Handler
{
    public interface IDiffGenerator
    {
        /// <summary>
        /// Updates diff results into a <see cref="DiffRecord"/>, regardless if it already exists or not.
        /// </summary>
        /// <param name="diffRecord">The <see cref="DiffRecord"/> instance to be updated.</param>
        /// <remarks>
        /// Pre-conditions (otherwise this method will silently exit):<br/>
        /// - <see cref="DiffRecord.IsComplete"/> == true<br/>
        /// - <see cref="DiffRecord.Left"/> and <see cref="DiffRecord.Right"/> must have same length.
        /// </remarks>
        void RefreshDiff(DiffRecord diffRecord);
    }
}
