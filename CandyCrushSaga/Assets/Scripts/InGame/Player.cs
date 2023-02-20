using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    StateComponent stateComponent;

    // Start is called before the first frame update
    void Start()
    {
        stateComponent = new StateComponent();
        stateComponent.CreateState("Attack", AttackUpdate, AttackStart, AttackEnd);
        stateComponent.CreateState("Death", DeathUpdate, DeathStart, DeathEnd);
        stateComponent.ChangeState("Attack");
    }

    float LifeTime;

    // Update is called once per frame
    void Update()
    {
        stateComponent.StateUpdate();

        LifeTime += Time.deltaTime;

        if (5.0f <= LifeTime)
        {
            LifeTime = 0.0f;
            stateComponent.ChangeState("Death");
        }
    }

    void AttackStart()
    {
        Debug.Log("AttackStart");
    }

    void AttackUpdate()
    {
        Debug.Log("AttackUpdate");
    }

    void AttackEnd()
    {
        Debug.Log("AttackEnd");
    }

    void DeathStart()
    {
        Debug.Log("DeathStart");
    }

    void DeathUpdate()
    {
        Debug.Log("DeathUpdate");
    }

    void DeathEnd()
    {
        Debug.Log("DeathEnd");
    }
}
