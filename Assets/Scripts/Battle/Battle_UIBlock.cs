using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_UIBlock : MonoBehaviour
{
    public Animator anim;
    public AudioClip bump;

    public string action = "Jump";
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        anim.Play("Hit");
        SoundManager.instance.Play(bump);


        
        BattleManager.instance.StartCoroutine(BattleManager.instance.PlayerAction(action));
    }
}
