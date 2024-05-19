using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BlockContainerType { 
    Coin,
    Breakable,
    P1,
    P2
}
public class BlockContainer : MonoBehaviour
{
    public BlockContainerType type;
    public string triggerIdentifier;

    public int sPlayerID = 0;

    bool hit = false;
    
    public Animator selfAnim;
    public GameObject selfParticle;
    public Vector3 selfVectorOffset;
    public int containerCount;
    public AudioClip hitClip;
    public AudioClip secondaryHitClip;
    bool hitting = false;


    public void Hit(int playerID) {

        if (this.sPlayerID == 1 && playerID != 0) return;
        if (this.sPlayerID == 2 && playerID != 1) return;


        if (selfAnim != null) { if (selfAnim.GetCurrentAnimatorStateInfo(0).IsName("Hit")){ return; } }
        SoundManager.instance.Play(hitClip);

        if (selfAnim != null) { if (selfAnim.GetCurrentAnimatorStateInfo(0).IsName("Hit_Last")) { return; } }

        if (hit) return;
        if (hitting) return;
        if (selfParticle != null) Instantiate(selfParticle, this.transform.position + selfVectorOffset, Quaternion.identity);
        if (secondaryHitClip != null) SoundManager.instance.Play(secondaryHitClip);
        
      

        hitting = true;


       
        containerCount--;

        if (containerCount <= 0)
        {
            hit = true;
            if (selfAnim != null) { selfAnim.Play("Hit_Last"); }
        }
        else {
            if (selfAnim != null) { selfAnim.Play("Hit"); }
        }

        if (triggerIdentifier != "")
        {
            if (TriggerManager.instance.TriggerButton(triggerIdentifier.ToUpper())) return;
        }

        if (type == BlockContainerType.Coin) {
            Global.coin++;
        }else if (type == BlockContainerType.Breakable)
        {
            Destroy(this.gameObject);
        }
        hitting = false;
    }
}
