namespace EventSourcing.Domain;

public abstract class Event
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AggregateId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int Versao { get; set; }
}

public class ContaCriada : Event
{
    public string Titular { get; set; } = string.Empty;
}

public class DepositoEfetuado : Event
{
    public decimal Valor { get; set; }
}

public class SaqueEfetuado : Event
{
    public decimal Valor { get; set; }
}
