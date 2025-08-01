using EventSourcing.Domain;
using System.Text.Json;

namespace EventSourcing;

public class ContaRepository(EventStore eventStore, SnapshotStore snapshotStore)
{
    public Conta GetById(Guid id)
    {
        Conta conta;

        var snapshot = snapshotStore.GetSnapshot(id);
        if (snapshot != null)
        {
            conta = JsonSerializer.Deserialize<Conta>(snapshot.Data);
        }

        // Aplica eventos posteriores à versão do snapshot
        var eventos = eventStore.GetEvents(id);
        conta = Conta.LoadFromHistory(eventos);
        return conta;
    }

    public void Salvar(Conta conta)
    {
        var changes = conta.GetChanges().ToList();
        if (!changes.Any()) return;

        eventStore.SaveEvents(conta.Id, changes);

        // A cada 5 eventos salva snapshot (exemplo)
        if (conta.Versao % 5 == 0)
        {
            var snapshot = new Snapshot
            {
                AggregateId = conta.Id,
                Version = conta.Versao,
                Data = JsonSerializer.Serialize(conta)
            };

            snapshotStore.SaveSnapshot(snapshot);
        }

        conta.ClearChanges();
    }
}