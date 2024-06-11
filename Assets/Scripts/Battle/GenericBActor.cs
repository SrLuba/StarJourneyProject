using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public enum DefenseType { 
    None,
    Jump,
    Hammer
}

public enum Emotion { 
    Neutral,
    Angry,
    Serious,
    Worried,
    Joyful
}
public enum StatusBattleActor { 
    None,
    Idle,
    Sideplay,
    Defend,
    Think
}
public class GenericBActor : MonoBehaviour
{
    public BattleActorSO self;

    public Transform animHandle;
    public Animator animator;
    public Rigidbody rb;

    Vector3 ogScale;
    public Vector2 turnPosition, normalPosition, otherPlayerTurnPosition;
    public string animationState = "Idle";
    public string idleAnimation = "";

    public bool myTurn = false;
    public bool IAMOVING = false;
    bool initialized = false;

    public bool Grounded = false;

    bool animationInterrupt = false;

    public int tempAttackID = 1;

    public DefenseType currentPlayerDefense;

    public Emotion currentEmotion;
    public StatusBattleActor status = StatusBattleActor.Idle;

    int randomizedNumber = 0;

    int index = 0;
    public void ShuffleAnimations() { randomizedNumber = Random.Range(0, 2); animator.SetFloat("Random", (float)randomizedNumber); }
    void Start()
    {
        ogScale = animHandle.localScale;

        if (self.linkedChara.charaType == CharaType.Enemy) {
            idleAnimation = "Idle"; animationState = "Idle";

        }

        self.stats.ATTACK.ResetStat(self.level);
        self.stats.ENERGY.ResetStat(self.level);
        self.stats.ATTACK.ResetStat(self.level);
        self.stats.DEFENSE.ResetStat(self.level);
        self.stats.MAGICATTACK.ResetStat(self.level);
        self.stats.MAGICDEFENSE.ResetStat(self.level);
        self.stats.SPEED.ResetStat(self.level);
        self.stats.LUCK.ResetStat(self.level);

        currentEmotion = Emotion.Neutral;
    }

    public void PlayAnim(bool emotion, string animationName) {

        
        string result = "";

        if (emotion) { result = currentEmotion.ToString()+"_"+ animationName; }
        else { result = animationName; }

        Debug.Log("TRYPlaying" + result);
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(result)) return;
        Debug.Log("Playing" + result);
        animator.Play(result);
    }
    public IEnumerator Die()
    {
        this.self.dead = true;
        this.PlayAnim(false, "Dying");
        yield return new WaitForSeconds(0.1f);
        animationInterrupt = true;
        this.PlayAnim(false, "Die");
        SoundManager.instance.Play(this.self.dieVoiceClip);

        BattleManager.instance.bActors.Remove(self);
        if (this.self.linkedChara.charaType == CharaType.Enemy) {
            BattleManager.instance.enemyActors.Remove(self);
        }


        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        this.enabled = false;
    }
    public IEnumerator Hurt() {
        if (!this.self.dead) { 
            animationInterrupt = true;
            this.PlayAnim(true, "Hurt");
            yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
            animationInterrupt = false;
        }
    }
    public void SetOnFloor() {
        this.transform.position = new Vector3(this.transform.position.x, BattleManager.instance.assignedBattle.floorY + self.linkedChara.floorYOffset, this.transform.position.z);
    }
    public IEnumerator IA_Goto_Walk(Vector2 position, string endingAnimation) {
        if (!IAMOVING) { 
            if (Vector2.Distance(new Vector3(this.transform.position.x, this.transform.position.z), new Vector3(position.x, position.y)) < 0.6f) {
                IAMOVING = false;
                
                yield break; 
            }

            Debug.LogWarning("Goto Walk " + this.gameObject.name + " x " + position.x.ToString() + " z " + position.y.ToString());
            while (Vector2.Distance(new Vector3(this.transform.position.x, this.transform.position.z), new Vector3(position.x, position.y)) > 0.6f) {
                this.PlayAnim(true, (Grounded) ? "Walk" : (rb.velocity.y > 0f) ? "Jump" : "Fall");
                IAMOVING = true; 
                this.transform.position = new Vector3(Mathf.MoveTowards(this.transform.position.x, position.x, self.moveSpeed*Time.deltaTime), this.transform.position.y, Mathf.MoveTowards(this.transform.position.z, position.y, self.moveSpeed * Time.deltaTime));
                yield return new WaitForSeconds(0.001f);
            }

            yield return new WaitForSeconds(0.001f);
            IAMOVING = false;
            this.PlayAnim(true, "Idle");
        }
    }
    public void UpdateGrounded() {
        Grounded = Physics.Raycast(this.transform.position, Vector3.down, 1.75f, self.linkedChara.floorMask);
        rb.velocity = new Vector3(0f,rb.velocity.y, 0f);

        if (self.stats.HEALTH.currentValue <= 0 && !this.self.dead) StartCoroutine(Die());
    }
    public void Jump() {
        rb.velocity = new Vector3(rb.velocity.x, self.jumpForce, rb.velocity.z);
        SoundManager.instance.Play(self.linkedChara.OVActor.jumpSFX);
    }
    public bool sidePlayInterrupt = false;

    public void UpdateSidePlays() {
        if (BattleManager.instance.currentTurn == null) {
            sidePlayInterrupt = false;
            return;
        }
        if (BattleManager.instance.currentTurn == this.self)
        {
            sidePlayInterrupt = false;
            return;
        }

        if (BattleManager.instance.currentTurn.linkedChara.charaType != CharaType.Player)
        {
            sidePlayInterrupt = false;
            return;
        }

        if (currentPlayerDefense != DefenseType.None)
        {
            sidePlayInterrupt = false;
            return;
        }

        sidePlayInterrupt = true;
        status = StatusBattleActor.Sideplay;
    }
    public bool strengthLoose = false;

    float strengthTimer = 1f;

    bool hammerlift = false;
    public IEnumerator Hammer() {
        if (hammerlift) {
            yield break;
        }
        hammerlift = true;
        animationInterrupt = true;
        this.PlayAnim(false, "HammerLift");
        SoundManager.instance.Play(BattleManager.instance.liftHammerSFX);
        yield return new WaitForSeconds(0.1f);

        strengthTimer = this.self.getHammerStrength();

        while (self.linkedChara.getActionDown() && !strengthLoose) {
            yield return new WaitForSeconds(0.1f);
            strengthTimer -= 0.1f;
            if (strengthTimer <= 0F) strengthLoose = true;
        }

        yield return new WaitForSeconds(0.1f);
        this.PlayAnim(false, "HammerReleas" + (string)((strengthLoose) ? "LOSE" : "e"));
        SoundManager.instance.Play(BattleManager.instance.releaseHammerSFX);
        hammerlift = false;
        yield return new WaitForSeconds(0.5f);
        animationInterrupt = false;
        strengthLoose = false;
     
    }

    public void Update()
    {
        UpdateGrounded();

        if (self.linkedChara.charaType == CharaType.Player) {
            if (self.linkedChara.getActionPress() && Grounded) {
                if (!BattleManager.instance.inputOverride) {

                    if (currentPlayerDefense == DefenseType.Jump || currentPlayerDefense == DefenseType.None) { Jump(); } 
                }
            }
            if (currentPlayerDefense == DefenseType.Hammer)
            {
                sidePlayInterrupt = false;
                if (self.linkedChara.getActionPress() && Grounded)
                {
                    StartCoroutine(Hammer());
                }
            }
            
        }

    }

    public void PrepareForTurn(BattleActorSO character) {

        if (!initialized)
        {
            if (self.linkedChara.charaType == CharaType.Player)
            {
                normalPosition = BattleManager.instance.getPlayerPos(0, self.linkedChara.identifier);
                turnPosition = BattleManager.instance.getPlayerPos(1, self.linkedChara.identifier);
                otherPlayerTurnPosition = normalPosition - new Vector2(1f, 0f);
            }
            initialized = true;
        }
        
       // animationInterrupt = false;
        currentPlayerDefense =  (character.linkedChara.charaType == CharaType.Enemy)  ? (DefenseType)(character.getInstance().GetComponent<GenericBActor>().tempAttackID+1) : DefenseType.None;

        status = (this.self == character) ? StatusBattleActor.Think : StatusBattleActor.Defend;

        Debug.Log("AAAAA");
        if (self.linkedChara.charaType == CharaType.Player) StartCoroutine(IA_Goto_Walk((this.self != character) ? ((character.linkedChara.charaType != CharaType.Player) ? normalPosition : otherPlayerTurnPosition)  : turnPosition, idleAnimation));
    }
    public void UpdateAnimation()
    {
        if (this.self.dead || animationInterrupt) return;

        if (status == StatusBattleActor.Idle || (status == StatusBattleActor.None))
        {
            if (!Grounded)
            {
                this.PlayAnim(false, (rb.velocity.y > 0f) ? "Jump" : "Fall");
            }
            else
            {
                this.PlayAnim(false, (IAMOVING) ? "Idle" : "Walk");
            }
        }
        else if (status == StatusBattleActor.Sideplay)
        {
            if (!Grounded)
            {
                this.PlayAnim(false, (rb.velocity.y > 0f) ? "Jump" : "Fall");
            }
            else
            {
                this.PlayAnim(true, (IAMOVING) ? "Walk" : "SidePlay");
            }
        }
        else if (status == StatusBattleActor.Think)
        {
            if (!Grounded)
            {
                this.PlayAnim(false, (rb.velocity.y > 0f) ? "Jump" : "Fall");
            }
            else
            {
                this.PlayAnim(true, (IAMOVING) ? "Walk" : "Think");
            }
        }
        else if (status == StatusBattleActor.Defend)
        {
            if (!Grounded)
            {
                this.PlayAnim(false, (rb.velocity.y > 0f) ? "Jump" : "Fall");
            }
            else
            {
                this.PlayAnim(true, (IAMOVING) ? "Walk" : "Defend");
            }
        }
        
    }

    void FixedUpdate()
    {
        if (self.linkedChara.charaType == CharaType.Enemy) animator.speed = (MusicManager.instance.myMusic.bpm / 60);

        self.stats.ATTACK.UpdateStat(self.level);
        self.stats.ENERGY.UpdateStat(self.level);
        self.stats.ATTACK.UpdateStat(self.level);
        self.stats.DEFENSE.UpdateStat(self.level);
        self.stats.MAGICATTACK.UpdateStat(self.level);
        self.stats.MAGICDEFENSE.UpdateStat(self.level);
        self.stats.SPEED.UpdateStat(self.level);
        self.stats.LUCK.UpdateStat(self.level);

      
        UpdateSidePlays();
        UpdateAnimation();
    }

}
