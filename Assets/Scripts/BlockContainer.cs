using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BlockContainerType { 
    Coin,
    Breakable
}
public class BlockContainer : MonoBehaviour
{
    public string triggerIdentifier;

    bool hit = false;
    public BlockContainerType type;
    public Animator selfAnim;
    public GameObject selfParticle;
    public Vector3 selfVectorOffset;
    public int containerCount;
    public AudioClip hitClip;
    public AudioClip secondaryHitClip;
    bool hitting = false;
    public void Hit() {
        

        if (selfAnim != null) { if (selfAnim.GetCurrentAnimatorStateInfo(0).IsName("Hit")){ return; } }
        SoundManager.instance.Play(hitClip);

        if (selfAnim != null) { if (selfAnim.GetCurrentAnimatorStateInfo(0).IsName("Hit_Last")) { return; } }

        if (hit) return;
        if (hitting) return;

        hitting = true;


        if (selfParticle!=null) Instantiate(selfParticle, this.transform.position + selfVectorOffset, Quaternion.identity);
        if (secondaryHitClip!=null) SoundManager.instance.Play(secondaryHitClip);
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
