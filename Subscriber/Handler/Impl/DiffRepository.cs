using System;
using System.Linq;
using LiteDB;

namespace Kafka.Diff.Subscriber.Handler.Impl
{
    /// <summary>
    /// Saves and lodas <see cref="DiffRecord"/> from a LocalDB repository
    /// </summary>
    public class DiffRepository : IDiffRepository
    {
        private readonly LiteCollection<DiffRecord> _col;

        // TODO: inject
        public const string DBPath = "./kafka-diff.db";

        public DiffRepository()
        {
            BsonMapper.Global.Entity<DiffRecord>()
                .Id(r => r.Id);

            var db = new LiteDatabase(DBPath);

            _col = db.GetCollection<DiffRecord>("records");
        }

        public void Save(DiffRecord record)
        {
            _col.Upsert(record);
        }

        public DiffRecord Load(Guid id)
        {
            var record = _col.Find(r => r.Id == id);

            return record?.FirstOrDefault();
        }
    }
}
