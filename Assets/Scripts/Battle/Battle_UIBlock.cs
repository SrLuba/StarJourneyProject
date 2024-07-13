using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_UIBlock : MonoBehaviour
{
    public Animator anim;
    public AudioClip bump;


    public AttackSO attack;
    public string action = "Jump";

    public bool hit = false;
    public bool targetting = false;


    public void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;
        if (BattleManager.instance.currentTurn == null) return;
        if (BattleManager.instance.currentTurn.linkedActor.identifier != other.GetComponent<GenericBActor>().self.linkedActor.identifier) return;
        if (hit) return;

        anim.Play("Hit");
        SoundManager.instance.Play(bump);
        hit = true;

        if (targetting) { BattleManager.instance.StartCoroutine(BattleManager.instance.Targetting(other.GetComponent<GenericBActor>().self, attack, action)); } else { BattleManager.instance.StartCoroutine(BattleManager.instance.PlayerAction(other.GetComponent<GenericBActor>().self, attack, action)); }
    }
}
