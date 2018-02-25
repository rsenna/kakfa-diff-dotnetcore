namespace Kakfka.Diff.Subscriber.Handler.Impl
{
    public class CacheRecord
    {
        public string Id { get; }
        public string Left { get; }
        public string Right { get; }
        public string Diff { get; }

        public CacheRecord(string id, string left = null, string right = null, string diff = null)
        {
            Id = id;
            Left = left;
            Right = right;
            Diff = diff;
        }

        public CacheRecord With(string newLeft = null, string newRight = null, string newDiff = null)
            => new CacheRecord(Id, newLeft ?? Left, newRight ?? Right, newDiff ?? Diff);

        public bool IsFull => Left != null && Right != null;

        public override string ToString() => $"Id: {Id} Left: {Left} Right: {Right} Diff: {Diff}.";
    }
}
