using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAttackAnimator : MonoBehaviour
{

    public Animator anim;

    public int currentTrack = 0;
    public int numberOfTracks = 2;

    public int accuracy = 0;

    public int damage = 1;

    public bool canTime = false;
    public bool timedCorrectly = false;

    public BattleActorSO player, target;

    public int timestampID;



    public void AccuracySet(int ammount) { 
       this.accuracy = ammount; 
    }
    public void TimestampStart(int id) {
        this.timestampID = id;
        this.canTime = true;
        this.timedCorrectly = false;
    }

    public void TimestampStop(int id) {
        Debug.Log("RANKINGID | " + Global.rankingID.ToString());
        if (this.timedCorrectly)
        {
            if (this.player == null && this.target == null) return;
            

        }
        else {
            anim.Play("Fail_" + id.ToString());
            Global.rankingID = 0;
            BattleManagerNumbers.instance.ShowRanking(Global.rankingID);
        }
        BattleManagerNumbers.instance.Hurt(BattleUtils.DamageGet(this.player, this.target), this.target);
        
        currentTrack++;
        this.canTime = false;
        this.timedCorrectly = false;
    }

    public void Reset()
    {
        currentTrack = 0;
        Global.rankingID = 0;
        this.canTime = false;
    }
    public void Hit() {
        if (this.player == null && this.target == null) return;
      

        if (this.target.dead)
        {
            anim.Play("DeadSuccess", 0, 0f);
        }
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
        if (!canTime && player.linkedActor.getKey(KeyEventType.Pressed)) { this.timedCorrectly = false; }
        if (canTime && player.linkedActor.getKey(KeyEventType.Pressed)) {
            currentTrack++;

            this.timedCorrectly = true;

            if (currentTrack >= numberOfTracks)
            {
             
            }
            else {
                if (this.target.stats.HEALTH.currentValue - BattleUtils.DamageGet(this.player, this.target) <= 0)
                {
                    BattleManagerNumbers.instance.Hurt(BattleUtils.DamageGet(this.player, this.target), this.target);
                    anim.Play("DeadSuccess", 0, 0f);
                }
                
               
            }
        
            Global.rankingID += accuracy;
            BattleManagerNumbers.instance.ShowRanking(Global.rankingID);
            canTime = false;
        }

    }
}
