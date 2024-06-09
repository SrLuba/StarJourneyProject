using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public enum DefenseType { 
    None,
    Jump,
    Hammer
}
public class GenericBActor : MonoBehaviour
{
    public CharaSO self;

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

    void Start()
    {
        ogScale = animHandle.localScale;

        if (self.charaType == CharaType.Enemy) {
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

    }
    public IEnumerator Die()
    {
        this.self.selfBattle.dead = true;
        animator.Play("Dying");
        yield return new WaitForSeconds(0.1f);
        animationInterrupt = true;
        animator.Play("Die");
        SoundManager.instance.Play(this.self.selfBattle.dieVoiceClip);
   
  
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        this.enabled = false;
    }
    public IEnumerator Hurt() {
        if (!this.self.selfBattle.dead) { 
            animationInterrupt = true;
            animator.Play("Hurt");
            yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
            animationInterrupt = false;
        }
    }
    public void SetOnFloor() {
        this.transform.position = new Vector3(this.transform.position.x, BattleManager.instance.assignedBattle.floorY + self.floorYOffset, this.transform.position.z);
    }
    public IEnumerator IA_Goto_Walk(Vector2 position, string endingAnimation) {
        if (!IAMOVING) { 
            if (Vector2.Distance(new Vector3(this.transform.position.x, this.transform.position.z), new Vector3(position.x, position.y)) < 0.6f) {
                IAMOVING = false;
                yield break; 
            }

            Debug.LogWarning("Goto Walk " + this.gameObject.name + " x " + position.x.ToString() + " z " + position.y.ToString());
            while (Vector2.Distance(new Vector3(this.transform.position.x, this.transform.position.z), new Vector3(position.x, position.y)) > 0.6f) {
                animationState = "Walk";
                IAMOVING = true;
                this.transform.position = new Vector3(Mathf.MoveTowards(this.transform.position.x, position.x, self.selfBattle.moveSpeed*Time.deltaTime), this.transform.position.y, Mathf.MoveTowards(this.transform.position.z, position.y, self.selfBattle.moveSpeed * Time.deltaTime));
                yield return new WaitForSeconds(0.001f);
            }

            yield return new WaitForSeconds(0.001f);
            IAMOVING = false;
            animationState = endingAnimation;
        }
    }
    public void UpdateGrounded() {
        Grounded = Physics.Raycast(this.transform.position, Vector3.down, 1.75f, self.floorMask);
        rb.velocity = new Vector3(0f,rb.velocity.y, 0f);

        if (self.stats.HEALTH.currentValue <= 0 && !this.self.selfBattle.dead) StartCoroutine(Die());
    }
    public void Jump() {
        rb.velocity = new Vector3(rb.velocity.x, self.selfBattle.jumpForce, rb.velocity.z);
        SoundManager.instance.Play(self.OVActor.jumpSFX);
    }

    public IEnumerator Hammer() { 
        animationInterrupt = true;
        animator.Play("HammerLift");
        SoundManager.instance.Play(BattleManager.instance.liftHammerSFX);
        yield return new WaitForSeconds(0.1f);


        while (self.getActionDown()) {
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.1f);
        animator.Play("HammerRelease");
        SoundManager.instance.Play(BattleManager.instance.releaseHammerSFX);
    }

    public void Update()
    {
        UpdateGrounded();

        if (self.charaType == CharaType.Player) {
            if (self.getActionPress() && Grounded) {
                if (!BattleManager.instance.inputOverride) {

                    if (currentPlayerDefense == DefenseType.Jump || currentPlayerDefense == DefenseType.None) { Jump(); } 
                }
            }
            if (currentPlayerDefense == DefenseType.Hammer)
            {
                if (self.getActionPress() && Grounded)
                {
                    StartCoroutine(Hammer());
                }
            }
            
        }

    }

    public void PrepareForTurn(CharaSO character) {
        if (this.self.charaType != CharaType.Player) {
           if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walk")) animator.Play("Idle");
            return;
        }

        if (!initialized)
        {
            if (self.charaType == CharaType.Player)
            {
                normalPosition = BattleManager.instance.getPlayerPos(0, self.identifier);
                turnPosition = BattleManager.instance.getPlayerPos(1, self.identifier);
                otherPlayerTurnPosition = normalPosition - new Vector2(1f, 0f);
            }
            initialized = true;
        }

        currentPlayerDefense =  (character.charaType == CharaType.Enemy)  ? (DefenseType)(character.selfBattle.getInstance().GetComponent<GenericBActor>().tempAttackID+1) : 0;

        idleAnimation = (this.self == character) ? self.selfBattle.getIdleAnim() : "Idle";
        StartCoroutine(IA_Goto_Walk((this.self != character) ? ((character.charaType != CharaType.Player) ? normalPosition : otherPlayerTurnPosition)  : turnPosition, idleAnimation));
    }

    void FixedUpdate()
    {
        self.stats.ATTACK.UpdateStat(self.level);
        self.stats.ENERGY.UpdateStat(self.level);
        self.stats.ATTACK.UpdateStat(self.level);
        self.stats.DEFENSE.UpdateStat(self.level);
        self.stats.MAGICATTACK.UpdateStat(self.level);
        self.stats.MAGICDEFENSE.UpdateStat(self.level);
        self.stats.SPEED.UpdateStat(self.level);
        self.stats.LUCK.UpdateStat(self.level);
        if (!this.self.selfBattle.dead)
        {
            if (!Grounded)
            {
                animationState = (rb.velocity.y > 0f) ? "Jump" : "Fall";
            }

            if (!animationInterrupt) { if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationState)) animator.Play(animationState); }

            if (IAMOVING) { animationState = "Walk"; }
            else
            {
                animationState = idleAnimation;
            }
            animHandle.localScale = Vector3.Lerp(animHandle.localScale, ogScale + new Vector3(Mathf.Clamp(rb.velocity.y * self.effectSquatch, self.xSquatch.x, self.xSquatch.y), Mathf.Clamp(rb.velocity.y * self.effectSquatchY, self.ySquatch.x, self.ySquatch.y), Mathf.Clamp(rb.velocity.y * self.effectSquatch, self.xSquatch.x, self.xSquatch.y)), 15f * Time.deltaTime);
        }
    }
}
