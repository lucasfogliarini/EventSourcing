namespace EventSourcing.Domain;

public class Conta
{
    private Conta() { }
    public Conta(string titular)
    {
        var evt = new ContaCriada { AggregateId = Guid.NewGuid(), Versao = 1, Titular = titular };
        Apply(evt);
        _changes.Add(evt);
    }
    public Guid Id { get; private set; }
    public int Versao { get; private set; }
    public string? Titular { get; private set; }
    public decimal Saldo { get; private set; }

    private readonly List<Event> _changes = [];

    public IEnumerable<Event> GetChanges() => _changes;

    public void Depositar(decimal valor)
    {
        var evt = new DepositoEfetuado { AggregateId = Id, Versao = Versao + 1, Valor = valor };
        Apply(evt);
        _changes.Add(evt);
    }

    public void Sacar(decimal valor)
    {
        if (valor > Saldo) throw new InvalidOperationException("Saldo insuficiente.");
        var evt = new SaqueEfetuado { AggregateId = Id, Versao = Versao + 1, Valor = valor };
        Apply(evt);
        _changes.Add(evt);
    }

    public void Apply(Event evt)
    {
        Versao = evt.Versao;
        switch (evt)
        {
            case ContaCriada e:
                Id = e.AggregateId;
                Titular = e.Titular;
                Saldo = 0;
                break;

            case DepositoEfetuado e:
                Saldo += e.Valor;
                break;

            case SaqueEfetuado e:
                Saldo -= e.Valor;
                break;
        }
    }

    public static Conta LoadFromHistory(IEnumerable<Event> history)
    {
        var conta = new Conta();
        foreach (var evt in history)
            conta.Apply(evt);
        return conta;
    }

    public void ClearChanges() => _changes.Clear();
}
