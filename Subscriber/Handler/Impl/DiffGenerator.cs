namespace Kafka.Diff.Subscriber.Handler.Impl
{
    public class DiffGenerator : IDiffGenerator
    {
        public string GetDiff(CacheRecord cacheRecord)
        {
            // TODO: return json
            // TODO: implement proper diff

            return
                $"Id: {cacheRecord.Id} Left Size: {cacheRecord.Left?.Length.ToString() ?? "(undefined)"} Right Size: {cacheRecord.Right?.Length.ToString() ?? "(undefined)."}";
        }
    }
}
