using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IEnemyState
{
    public void EnterState(EnemyStateManager enemy)
    {
        Debug.Log("[Chase State] : State Entered");
    }

    public void ExitState(EnemyStateManager enemy)
    {
        Debug.Log("[Chase State] : State Exited");
    }

    public void UpdateState(EnemyStateManager enemy)
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            enemy.TransitionToState(new AttackState());
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
