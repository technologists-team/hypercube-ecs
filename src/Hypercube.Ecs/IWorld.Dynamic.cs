namespace Hypercube.Ecs;

public partial interface IWorld
{
    public object Add(Entity entity, Type type);

    /// <summary>
    /// Dynamically adds a specific component instance to an entity.
    /// </summary>
    /// <param name="entity">The target entity.</param>
    /// <param name="value">The component instance to add. If null, the operation is ignored.</param>
    public void Add(Entity entity, object? value);

    /// <summary>
    /// Dynamically retrieves a component instance of the specified <paramref name="type"/> from an entity.
    /// </summary>
    /// <param name="entity">The target entity.</param>
    /// <param name="type">The runtime type of the component to retrieve.</param>
    /// <returns>The component instance.</returns>
    public object Get(Entity entity, Type type);

    /// <summary>
    /// Dynamically checks if an entity has a component of the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="entity">The target entity.</param>
    /// <param name="type">The runtime type of the component to check.</param>
    /// <returns>True if the entity possesses the component; otherwise, false.</returns>
    public bool Has(Entity entity, Type type);

    /// <summary>
    /// Dynamically removes a component of the specified <paramref name="type"/> from an entity.
    /// </summary>
    /// <param name="entity">The target entity.</param>
    /// <param name="type">The runtime type of the component to remove.</param>
    public void Remove(Entity entity, Type type);
}