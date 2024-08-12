using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// This script goes into the counter attack collider of the enemy.
public class CounterAttackEnemyScript : MonoBehaviour
{
    public int CounterAttackType = 0;
    public Battle_Enemy_Attack enemyAttackSelf;

    public void OnTriggerStay(Collider other)
    {
        if (other.tag != "Player") return;
        if (other.GetComponent<GenericBActor>().Grounded || other.GetComponent<GenericBActor>().stunned) { return; }

        if (CounterAttackType == 0)
        {
            if (other.GetComponent<Rigidbody>().velocity.y > 0f) { return; }
           
        }
        //enemyAttackSelf.CounterAttack(other.gameObject);
    }
}
