using Hypercube.Ecs.Entities;

namespace Hypercube.Ecs;

public partial class World
{
    private readonly EntityFactory _entityFactory = new();
    private EntityLocation[] _entityLocations = new EntityLocation[1024];
    
    public Entity Create()
    {
        var entity = _entityFactory.Create();
        EnsureEntityLocationCapacity(entity.Id);
        
        // Add to empty archetype
        var (chunk, index) = _emptyArchetype.AddEntity(entity);
        
        // Find chunk index
        var chunkIndex = 0;
        foreach (var c in _emptyArchetype.Chunks)
        {
            if (c == chunk)
                break;
            
            chunkIndex++;
        }

        _entityLocations[entity.Id] = new EntityLocation(0, chunkIndex, index);
        
        return entity;
    }

    public void Delete(Entity entity)
    {
        if (!_entityFactory.Validate(entity))
            return;

        // Remove from current archetype
        if (entity.Id < _entityLocations.Length)
        {
            var location = _entityLocations[entity.Id];
            if (location.ArchetypeIndex < _archetypeCount)
            {
                var archetype = _archetypes[location.ArchetypeIndex];
                var chunk = archetype.Chunks[location.ChunkIndex];
                archetype.RemoveEntity(chunk, location.LocalIndex);
            }
        }
        
        // Legacy: remove from pools
        foreach (var pool in _pools.Values)
            pool.Remove(entity);

        _entityFactory.Delete(entity);
    }

    public bool Validate(Entity entity)
        => _entityFactory.Validate(entity);

    /// <summary>
    /// Gets the current version for an entity id. Used by QueryEnumerator to construct valid Entity.
    /// </summary>
    internal int GetEntityVersion(int entityId)
    {
        if (entityId < 0 || entityId >= _entityFactory.Versions.Length)
            return -1;
        
        return _entityFactory.Versions[entityId];
    }
}