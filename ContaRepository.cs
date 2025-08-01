public class ContaRepository
{
    private readonly EventStore _eventStore;
    private readonly SnapshotStore _snapshotStore;

    public ContaRepository(EventStore eventStore, SnapshotStore snapshotStore)
    {
        _eventStore = eventStore;
        _snapshotStore = snapshotStore;
    }

    public async Task<Conta> GetById(Guid id)
    {
        var conta = new Conta();

        // Carrega snapshot se existir
        var snapshot = await _snapshotStore.GetSnapshot(id);
        if (snapshot != null)
        {
            conta = System.Text.Json.JsonSerializer.Deserialize<Conta>(snapshot.Data);
        }

        // Aplica eventos posteriores à versão do snapshot
        var eventos = await _eventStore.GetEvents(id);
        conta.LoadFromHistory(eventos.Where(e => e.Version > conta.Versao));

        return conta;
    }

    public async Task Save(Conta conta)
    {
        var changes = conta.GetChanges().ToList();
        if (!changes.Any()) return;

        await _eventStore.SaveEvents(conta.Id, changes);

        // A cada 5 eventos salva snapshot (exemplo)
        if (conta.Versao % 5 == 0)
        {
            var snapshot = new Snapshot
            {
                AggregateId = conta.Id,
                Version = conta.Versao,
                Data = System.Text.Json.JsonSerializer.Serialize(conta)
            };

            await _snapshotStore.SaveSnapshot(snapshot);
        }

        conta.ClearChanges();
    }
}
