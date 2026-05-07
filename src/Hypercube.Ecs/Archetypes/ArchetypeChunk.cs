using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Hypercube.Ecs.Archetypes;

/// <summary>
/// A chunk of entities that share the same archetype.
/// Stores component data in column-oriented format for cache efficiency.
/// Uses arrays for minimal allocations.
/// </summary>
[PublicAPI]
public sealed class ArchetypeChunk
{
    public readonly Archetype Archetype;
    public readonly int Capacity;

    public int Count { get; private set; }

    private readonly int[] _entities;
    
    public ReadOnlySpan<int> Entities => new(_entities, 0, Count);
    public bool Full => Count >= Capacity;
    
    public ArchetypeChunk(Archetype archetype, int capacity)
    {
        Archetype = archetype;
        Capacity = capacity;
        
        _entities = new int[capacity];
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int AddEntity(Entity entity)
    {
        var index = Count++;
        _entities[index] = entity.Id;
        
        return index;
    }
    
    public int RemoveEntity(int index)
    {
        var lastIndex = --Count;

        if (index == lastIndex)
            return -1;

        var movedEntity = _entities[lastIndex];
        _entities[index] = movedEntity;

        return movedEntity;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator() => new(this);
    
    [PublicAPI]
    public ref struct Enumerator
    {
        private readonly ArchetypeChunk _chunk;
        private readonly bool _initialized;
        
        private int _index;
        
        public int CurrentEntityId => _chunk._entities[_index];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator(ArchetypeChunk chunk)
        {
            _chunk = chunk;
            _index = -1;
            _initialized = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => _initialized && ++_index < _chunk.Count;
    }
}
