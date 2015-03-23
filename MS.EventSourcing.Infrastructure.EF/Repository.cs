using System;
using System.Collections.Generic;
using System.Linq;
using MS.EventSourcing.Infrastructure.EF.Models;

namespace MS.EventSourcing.Infrastructure.EF
{
    /// <summary>
    /// Represents event stream and snapshot persistance for MySql over EF6
    /// </summary>
    public class Repository : IRepository
    {
        private bool _disposed;
        private EventStoreContext _context;

        public static class StringResources
        {
            public static Func<string> ErrRepositoryNotInitialized = () => "MySql repository has not been initialized properly!";
        }

        /// <summary>
        /// Initializes the repository with a specific context information (default: "Name=EventStoreContext").
        /// </summary>
        public void Initialize(string contextInfo = null)
        {
            if (_context != null) return;
            _context = string.IsNullOrWhiteSpace(contextInfo) ? new EventStoreContext() : new EventStoreContext(contextInfo);
        }

        private void EnsureContext()
        {
            if (_context == null)
            {
                throw new InvalidOperationException(StringResources.ErrRepositoryNotInitialized());
            }
        }

        public IEnumerable<EventStream> GetEventStream(Guid aggregateRoodId, string aggregateType, long startVersion = 0)
        {
            EnsureContext();
            return (from eventStream in _context.EventStreams
                where
                    ((eventStream.AggregateRootId == aggregateRoodId)
                    && (eventStream.AggregateType == aggregateType)
                    && (eventStream.SequenceStart >= startVersion))
                orderby eventStream.SequenceStart
                select eventStream)
                .ToList();
        }

        public void InsertEvents(EventStream eventStream)
        {
            EnsureContext();
            _context.EventStreams.Add(eventStream);
            _context.SaveChanges();
        }

        public SnapshotStream GetSnapshot(Guid aggregateRootId, string snapshotType)
        {
            EnsureContext();
            return (from snapshotStream in _context.SnapshotStreams
                where ((snapshotStream.AggregateRootId == aggregateRootId) 
                && (snapshotStream.SnapshotType == snapshotType))
                orderby snapshotStream.Version descending
                select snapshotStream
                ).FirstOrDefault();
        }

        public void InsertSnapshot(SnapshotStream snapshot)
        {
            EnsureContext();
            _context.SnapshotStreams.Add(snapshot);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Repository()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only
                if (_context != null) _context.Dispose();
            }

            // release any unmanaged objects
            // set thick object references to null
            _context = null;
            _disposed = true;
        }
    }
}
