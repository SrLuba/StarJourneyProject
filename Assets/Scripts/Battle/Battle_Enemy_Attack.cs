using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_Enemy_Attack : MonoBehaviour
{
    public BattleActorSO self;
    public Animator selfAnimator;
    public string counterAttackAnimationName = "Counter";

    public bool counterAttack = false;

    public void CounterAttack(GenericBActor player) {
        if (counterAttack) return;
        counterAttack = true;
        selfAnimator.Play(counterAttackAnimationName, 0, 0f);
        player.GetComponent<Rigidbody>().velocity = new Vector3(player.GetComponent<Rigidbody>().velocity.x, 25f, player.GetComponent<Rigidbody>().velocity.z);
        BattleManagerNumbers.instance.Hurt(BattleUtils.DamageGet(player.self, this.self), this.self);
    }
    public void End() {
        Destroy(this.gameObject);
    }
}
