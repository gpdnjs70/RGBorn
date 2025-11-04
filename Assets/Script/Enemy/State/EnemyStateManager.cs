using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{
    public IEnemyState CurrentState;

    // Start is called before the first frame update
    void Start()
    {
        TransitionToState(new IdleState());
    }

    // Update is called once per frame
    void Update()
    {
        CurrentState?.UpdateState(this);
    }

    public void TransitionToState(IEnemyState newState)
    {
        CurrentState?.ExitState(this);
        CurrentState = newState;
        CurrentState.EnterState(this);
        print($"[TransitionToState] State transitioned to {newState}");
    }
}
