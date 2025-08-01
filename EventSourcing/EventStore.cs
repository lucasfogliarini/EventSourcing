using EventSourcing.Domain;

namespace EventSourcing;

public class EventStore
{
    private readonly List<Event> _eventDb = [];

    public void SaveEvents(Guid aggregateId, IEnumerable<Event> events) => _eventDb.AddRange(events);

    public IEnumerable<Event> GetEvents(Guid aggregateId)
    {
        return _eventDb
            .Where(e => e.AggregateId == aggregateId)
            .OrderBy(e => e.Versao);
    }
}

public class Snapshot
{
    public Guid AggregateId { get; set; }
    public int Version { get; set; }
    public string? Data { get; set; } // JSON do estado
}

public class SnapshotStore
{
    private readonly List<Snapshot> _snapshots = [];

    public void SaveSnapshot(Snapshot snapshot)
    {
        _snapshots.RemoveAll(s => s.AggregateId == snapshot.AggregateId);
        _snapshots.Add(snapshot);
    }

    public Snapshot? GetSnapshot(Guid aggregateId)
    {
        return _snapshots.FirstOrDefault(s => s.AggregateId == aggregateId);
    }
}