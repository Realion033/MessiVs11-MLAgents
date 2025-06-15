using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    private PlayerStateMachine _playerStateMachine;

    void Awake()
    {
        _playerStateMachine = new PlayerStateMachine(this);
    }

    void Update()
    {
        _playerStateMachine.Update();
    }
}
