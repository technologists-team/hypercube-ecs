using System.Diagnostics;
using System.Runtime.CompilerServices;
using Hypercube.Ecs.Components;
using Hypercube.Utilities.Collections.Bit;
using JetBrains.Annotations;

namespace Hypercube.Ecs.Archetypes;

/// <summary>
/// Represents a unique combination of components (archetype).
/// Entities with the same set of components belong to the same archetype.
/// Uses arrays instead of lists to minimize allocations.
/// </summary>
[PublicAPI]
[DebuggerDisplay("{Signature.ComponentNames}")]
public sealed class Archetype
{
    public const int ChunkCapacity = 256;
    
    public readonly Signature Signature;
    public readonly BitSet BitSet;
    public readonly int HashCode;
    
    private ArchetypeChunk[] _chunks = new ArchetypeChunk[4];
    private ArchetypeChunk? _lastChunk;
    private int _chunkCount;

    public int EntityCount { get; private set; }
    
    public ReadOnlySpan<ArchetypeChunk> Chunks => new(_chunks, 0, _chunkCount);
    
    public Archetype(Signature signature)
    {
        Signature = signature;
        BitSet = signature.AsBitSet();
        
        HashCode = BitSet.GetHashCode();
    }

    /// <summary>
    /// Adds an entity to this archetype, returning the chunk and local index.
    /// </summary>
    public ChunkEntity AddEntity(Entity entity)
    {
        if (_lastChunk is null || _lastChunk.Full)
        {
            EnsureChunkCapacity();
            
            _lastChunk = new ArchetypeChunk(this, ChunkCapacity);
            _chunks[_chunkCount++] = _lastChunk;
        }

        var index = _lastChunk.AddEntity(entity);
        EntityCount++;
        
        return new ChunkEntity(_lastChunk, index);
    }

    /// <summary>
    /// Removes an entity from this archetype.
    /// </summary>
    public int RemoveEntity(ArchetypeChunk chunk, int index)
    {
        EntityCount--;
        return chunk.RemoveEntity(index);
    }

    /// <summary>
    /// Returns an enumerator over all entities in this archetype.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator() => new(this);

    private void EnsureChunkCapacity()
    {
        if (_chunkCount < _chunks.Length)
            return;

        Array.Resize(ref _chunks, _chunks.Length * 2);
    }

    /// <summary>
    /// Fast enumerator over all entities in the archetype.
    /// </summary>
    [PublicAPI]
    public ref struct Enumerator
    {
        private readonly Archetype _archetype;
        private readonly bool _initialized;
        
        private int _chunkIndex;
        private ArchetypeChunk.Enumerator _chunkEnumerator;

        public int CurrentEntityId => _chunkEnumerator.CurrentEntityId;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator(Archetype archetype)
        {
            _archetype = archetype;
            _initialized = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            if (!_initialized)
                return false;
            
            while (true)
            {
                if (_chunkEnumerator.MoveNext())
                    return true;

                if (_chunkIndex >= _archetype._chunkCount)
                    return false;

                _chunkEnumerator = _archetype._chunks[_chunkIndex++].GetEnumerator();
            }
        }
    }
}