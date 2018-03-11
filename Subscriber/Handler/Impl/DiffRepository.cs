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

        public const string DBPath = "./kafka-diff.db";

        /// <summary>
        /// Constructor. Sets up LocalDB mappings.
        /// </summary>
        public DiffRepository()
        {
            BsonMapper.Global.Entity<DiffRecord>()
                .Id(r => r.Id);

            var db = new LiteDatabase(DBPath);

            _col = db.GetCollection<DiffRecord>("records");
        }

        /// <summary>
        /// Saves a <see cref="DiffRecord"/> into LocalDB.
        /// </summary>
        /// <param name="record">A <see cref="DiffRecord"/> instance.</param>
        public void Save(DiffRecord record)
        {
            _col.Upsert(record);
        }

        /// <summary>
        /// Loads a <see cref="DiffRecord"/> from LocalDB.
        /// </summary>
        /// <param name="id">A <see cref="Guid"/> representing the record primary key.</param>
        /// <returns>A <see cref="DiffRecord"/> if it exists, or null otherwise.</returns>
        public DiffRecord Load(Guid id)
        {
            var record = _col.Find(r => r.Id == id);

            return record?.FirstOrDefault();
        }
    }
}
