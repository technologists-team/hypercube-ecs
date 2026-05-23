using System.Runtime.InteropServices;
using Hypercube.Ecs.Utilities.Extensions;
using Hypercube.Utilities.Collections.Bit;
using Hypercube.Utilities.Helpers;
using JetBrains.Annotations;

namespace Hypercube.Ecs.Components;

/// <summary>
/// Represents a set of <see cref="ComponentMeta"/> that defines the composition of an entity.
/// </summary>
[PublicAPI]
public readonly struct Signature : IEquatable<Signature>
{
    public static readonly Signature Empty = new();
    
    private readonly ComponentMeta[] _components = [];
    private readonly int _hashCode;
    private readonly int _maxId;
    
    public int Count => _components.Length;
    public bool IsEmpty => this == Empty;
    public string ComponentNames => string.Join(';', _components.Select(meta => ComponentRegistry.ResolveType(meta).Name));
    
    public Signature()
    {
        _components = [];
        _hashCode = -1;
    }

    public Signature(params ComponentMeta[] components)
    {
        _components = components;
        
        foreach (ref var component in components.AsSpan())
        {
            if (component.Id > _maxId)
                _maxId = component.Id;
        }
        
        _hashCode = GetHashCode(_components);
    }

    public Signature(Span<ComponentMeta> components) : this(components.ToArray())
    {
    }

    public Signature(ReadOnlySpan<ComponentMeta> components) : this(components.ToArray())
    {
    }

    public Signature(HashSet<ComponentMeta> components) : this(components.ToArray())
    {
    }

    public Span<ComponentMeta> AsSpan() => MemoryMarshal.CreateSpan(ref _components[0], _components.Length);

    public BitSet AsBitSet() => GetBitSet(this);

    public bool Equals(Signature other) => _hashCode == other._hashCode;

    public override bool Equals(object? obj) => obj is Signature other && Equals(other);

    public override int GetHashCode() => _hashCode;
    
    public static Signature Add(Signature left, Signature right)
    {
        var set = new HashSet<ComponentMeta>(left.Count + right.Count);
        set.UnionWith(left._components);
        set.UnionWith(right._components);
        return new Signature(set.ToArray());
    }
    
    public static Signature Remove(Signature left, Signature right)
    {
        var set = new HashSet<ComponentMeta>(left.Count);
        set.UnionWith(left._components);
        set.ExceptWith(right._components);
        return new Signature(set.ToArray());
    }
    
    public int GetHashCode(Span<ComponentMeta> components)
    {
        Span<uint> stack = stackalloc uint[MemoryHelper.GetBitChunkCount(_maxId + 1)];
        var bitSet = new SpanBitSet<uint>(stack);

        foreach (ref var type in components)
            bitSet.Set(type.Id);

        return HashCodeHelper.HashFromSpan(bitSet.AsReadOnlySpan());
    }

    public BitSet GetBitSet(Signature signature)
    {
        return signature.Count == 0
            ? BitSet.Empty
            : new BitSet(_maxId + 1).Set(signature._components); // NOTE: It may be worth finding out the size of the chunk in advance
    }
    
    public static bool operator ==(Signature left, Signature right) => left.Equals(right);

    public static bool operator !=(Signature left, Signature right) => !(left == right);
    
    public static Signature operator +(Signature left, Signature right) => Add(left, right);
    
    public static Signature operator -(Signature left, Signature right) => Remove(left, right);
}