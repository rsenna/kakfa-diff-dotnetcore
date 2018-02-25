using System.Linq;
using LiteDB;

namespace Kakfka.Diff.Subscriber.Handler.Impl
{
    public class DiffRepository : IDiffRepository
    {
        private readonly LiteCollection<CacheRecord> _col;

        // TODO: inject
        public const string DBPath = "./kafka-diff.db";

        public DiffRepository()
        {
            BsonMapper.Global.Entity<CacheRecord>()
                .Id(r => r.Id);

            var db = new LiteDatabase(DBPath);

            _col = db.GetCollection<CacheRecord>("records");
        }

        public void Save(CacheRecord record)
        {
            // TODO: not sure if id works as string here
            _col.Upsert(record);
        }

        public CacheRecord Load(string id)
        {
            var record = _col.Find(r => r.Id == id);

            return record?.FirstOrDefault();
        }
    }
}
