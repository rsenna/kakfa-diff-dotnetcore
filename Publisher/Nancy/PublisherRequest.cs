namespace Kafka.Diff.Publisher.Nancy
{
    /// <summary>
    /// Request sent by the <see cref="SubmitController"/>
    /// </summary>
    public class PublisherRequest
    {
        public string Data { get; set; }
    }
}
