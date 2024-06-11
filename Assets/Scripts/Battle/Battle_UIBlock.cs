using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_UIBlock : MonoBehaviour
{
    public Animator anim;
    public AudioClip bump;


    public BattleAttackSO attack;
    public string action = "Jump";

    public bool hit = false;


    public void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;
        if (BattleManager.instance.currentTurn == null) return;
        if (BattleManager.instance.currentTurn.linkedChara.identifier != other.GetComponent<GenericBActor>().self.linkedChara.identifier) return;
        if (hit) return;

        anim.Play("Hit");
        SoundManager.instance.Play(bump);

        BattleManager.instance.StartCoroutine(BattleManager.instance.PlayerAction(other.GetComponent<GenericBActor>().self, attack, action));
        hit = true;
    }
}
