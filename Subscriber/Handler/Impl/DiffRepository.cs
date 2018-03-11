using System;
using System.Collections.Generic;
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
        public bool Save(DiffRecord record) =>
            _col.Upsert(record);

        /// <summary>
        /// Loads a <see cref="DiffRecord"/> from LocalDB.
        /// </summary>
        /// <param name="id">A <see cref="Guid"/> representing the record primary key.</param>
        /// <returns>A <see cref="DiffRecord"/> if it exists, or null otherwise.</returns>
        public DiffRecord Load(Guid id) =>
            _col.Find(r => r.Id == id)?.FirstOrDefault();

        /// <summary>
        /// Removes a record from the collection using the given <paramref name="id"/>.
        /// </summary>
        /// <param name="id">A <see cref="Guid"/> representing the record primary key.</param>
        /// <returns>True if the record was deleted, false if it was not found.</returns>
        public bool Delete(Guid id) =>
            _col.Delete(id);
    }
}
