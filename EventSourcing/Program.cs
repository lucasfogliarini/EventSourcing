using EventSourcing;
using EventSourcing.Domain;

var eventStore = new EventStore();
var snapshotStore = new SnapshotStore();
var repo = new ContaRepository(eventStore, snapshotStore);

var conta = new Conta("Lucas Fogliarini");
conta.Depositar(100);
conta.Sacar(30);

repo.Salvar(conta);

// Carregar estado reconstruído
var conta2 = repo.GetById(conta.Id);
Console.WriteLine($"Saldo final: {conta2.Saldo}");
