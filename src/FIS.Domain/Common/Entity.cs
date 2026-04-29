namespace FIS.Domain.Common;

/// <summary>
/// Clase base para entidades del dominio. Provee soporte para eventos de dominio
/// y un identificador genérico.
/// </summary>
/// <typeparam name="TKey">Tipo del identificador (int, long, Guid, etc.).</typeparam>
public abstract class Entity<TKey>
    where TKey : struct
{
    public TKey Id { get; protected set; }

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseEvent(DomainEvent ev) => _domainEvents.Add(ev);
    public void ClearEvents() => _domainEvents.Clear();
}

/// <summary>Marcador para eventos de dominio.</summary>
public abstract record DomainEvent(DateTime OccurredOn)
{
    protected DomainEvent() : this(DateTime.UtcNow) { }
}
