using Hypercube.Ecs.Components;
using Hypercube.Ecs.Events;
using Hypercube.Ecs.Queries;

namespace Hypercube.Ecs;

public partial interface IWorld
{
    IEventBus Events { get; }
    
    Entity Create();
    
    void Delete(Entity entity);
    
    bool Validate(Entity entity);
    
    ref T Add<T>(Entity entity)
        where T : struct, IComponent;
    ref T Add<T>(Entity entity, in T component)
        where T : struct, IComponent;
    
    ref T Get<T>(Entity entity)
        where T : struct, IComponent;
    
    bool Has<T>(Entity entity)
        where T : struct, IComponent;
    
    void Remove<T>(Entity entity)
        where T : struct, IComponent;
    
    Query Query(in QueryMeta meta);
}