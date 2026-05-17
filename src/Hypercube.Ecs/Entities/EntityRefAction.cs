using Hypercube.Ecs.Components;

namespace Hypercube.Ecs.Entities;

public delegate void EntityRefAction<T1>(Entity entity, ref T1 component1)
    where T1 : IComponent;
    
public delegate void EntityRefAction<T1, T2>(Entity entity, ref T1 component1, ref T2 component2) 
    where T1 : IComponent where T2 : IComponent;
    
public delegate void EntityRefAction<T1, T2, T3>(Entity entity, ref T1 component1, ref T2 component2, ref T3 component3) 
    where T1 : IComponent where T2 : IComponent where T3 : IComponent;

public delegate void EntityRefAction<T1, T2, T3, T4>(Entity entity, ref T1 component1, ref T2 component2, ref T3 component3,  ref T4 component4) 
    where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent;
