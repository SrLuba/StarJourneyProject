using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAttackAnimator : MonoBehaviour
{

    public Animator anim;

    public int currentTrack = 0;
    public int numberOfTracks = 2;

    public int damage = 1;

    public bool canTime = false;

    public BattleActorSO player, target;



    public void TimestampStart() {
        this.canTime = true; 
    }

    public void TimestampStop() {
        BattleManagerNumbers.instance.ShowRanking((Global.rankingID == 0) ?  0 : Global.rankingID + 1);
        currentTrack = 0;
        this.canTime = false; 
    }

    public void Reset()
    {
        currentTrack = 0;
        Global.rankingID = 0;
        this.canTime = false;
    }
    public void Hit() {
        if (this.player == null && this.target == null) return;
        BattleManagerNumbers.instance.Hurt(BattleUtils.DamageGet(this.player, this.target), this.target);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void EndAttack() {

        Destroy(this.gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        if (canTime && player.linkedActor.getKey(KeyEventType.Pressed)) {
            currentTrack++;
         
            if (currentTrack >= numberOfTracks)
            {
                BattleManagerNumbers.instance.ShowRanking(3);
                anim.Play("Success");
            }
            else { Global.rankingID++; anim.Play("Track_0" + currentTrack.ToString()); BattleManagerNumbers.instance.ShowRanking(Global.rankingID); }
            canTime = false;
        }

    }
}
