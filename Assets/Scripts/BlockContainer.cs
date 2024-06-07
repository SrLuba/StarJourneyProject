using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BlockContainer : MonoBehaviour
{

    public string triggerIdentifier;

    public int sPlayerID = 0;

    public List<PickupSO> pickupList;
    
    bool hit = false;
    
    public Animator selfAnim;
    public Vector3 selfVectorOffset;

    public AudioClip HitBlockClip;
 
    bool hitting = false;

    public void Start()
    {
        
    }
    public void Hit(int playerID) {

        if (this.sPlayerID == 1 && playerID != 0) return;
        if (this.sPlayerID == 2 && playerID != 1) return;

        SoundManager.instance.Play(HitBlockClip);
        if (selfAnim != null) { if (selfAnim.GetCurrentAnimatorStateInfo(0).IsName("Hit")){ return; } }
        

        if (selfAnim != null) { if (selfAnim.GetCurrentAnimatorStateInfo(0).IsName("Hit_Last")) { return; } }

        if (hit) return;
        if (hitting) return;
       
        
      

        hitting = true;

        if (pickupList.Count <= 1)
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

        if (pickupList.Count > 0) {
            PickupSO pickup = pickupList[0];
            InventoryManager.instance.AddItem(pickup, 1);

            Global.coin += pickup.moneyValue;
            
            if (pickup.selfParticle != null) Instantiate(pickup.selfParticle, this.transform.position + selfVectorOffset, Quaternion.identity);
            SoundManager.instance.Play(pickup.hitClip);
            if (pickup.secondaryHitClip != null) SoundManager.instance.Play(pickup.secondaryHitClip);
            

            pickupList.RemoveAt(0);

        }
        hitting = false;
    }
}
