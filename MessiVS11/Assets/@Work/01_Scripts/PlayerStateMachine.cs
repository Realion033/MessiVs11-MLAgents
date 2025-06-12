using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine
{
    private Entity _entity;
    private EntityBaseState _currentState;
    private EntityBaseState _previousState;

    public PlayerStateMachine(Entity entity)
    {
        _entity = entity;
        ChangeState(new PlayerIdleState(_entity));
    }

    public void Update()
    {
        _currentState.Update();
    }

    public void ChangeState(EntityBaseState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }
    
    public void RevertToPreviousState()
    {
        if (_previousState != null)
        {
            ChangeState(_previousState);
        }
    }
}
