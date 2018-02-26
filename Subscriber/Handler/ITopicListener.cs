namespace Kafka.Diff.Subscriber.Handler
{
    public interface ITopicListener
    {
        /// <summary>
        /// Read messages. When there is a left/right match, generate diff and save it in repository.
        /// </summary>
        void Process(int tries);
    }
}
