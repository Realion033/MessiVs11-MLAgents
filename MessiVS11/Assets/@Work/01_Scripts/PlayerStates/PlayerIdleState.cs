using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : EntityBaseState
{
    public PlayerIdleState(Entity entity) : base(entity)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Enter");
    }

    public override void Update()
    {
        base.Update();
        Debug.Log("Update");
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("Exit");
    }
}
