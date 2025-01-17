namespace CarnalContraption.Domain.Lovense;

public class Toy(ToyId id, ToyName name) : Entity<ToyId>(id)
{
    public ToyName Name { get; } = name;
}