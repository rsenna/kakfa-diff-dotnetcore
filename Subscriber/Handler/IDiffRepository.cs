using System;
using Kafka.Diff.Subscriber.Handler.Impl;

namespace Kafka.Diff.Subscriber.Handler
{
    /// <summary>
    /// Saves and lodas <see cref="DiffRecord"/> from a LocalDB repository
    /// </summary>
    public interface IDiffRepository
    {
        /// <summary>
        /// Saves a <see cref="DiffRecord"/> into LocalDB.
        /// </summary>
        /// <param name="record">A <see cref="DiffRecord"/> instance.</param>
        bool Save(DiffRecord record);

        /// <summary>
        /// Loads a <see cref="DiffRecord"/> from LocalDB.
        /// </summary>
        /// <param name="id">A <see cref="Guid"/> representing the record primary key.</param>
        /// <returns>A <see cref="DiffRecord"/> if it exists, or null otherwise.</returns>
        DiffRecord Load(Guid id);

        /// <summary>
        /// Removes a record from the collection using the given <paramref name="id"/>.
        /// </summary>
        /// <param name="id">A <see cref="Guid"/> representing the record primary key.</param>
        /// <returns>True if the record was deleted, false if it was not found.</returns>
        bool Delete(Guid id);
    }
}
