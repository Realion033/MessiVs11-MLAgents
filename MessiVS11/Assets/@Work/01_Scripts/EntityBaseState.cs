using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityBaseState : MonoBehaviour
{
    protected Entity _entity;
    
    public EntityBaseState(Entity entity)
    {
        _entity = entity;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() {}
}
