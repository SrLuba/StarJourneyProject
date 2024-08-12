using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;
using UnityEngine.UIElements;
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

    public bool animationInterrupt = false;

    public bool canJump = true;
    public bool stunned = false;
    public int tempAttackID = 0;

    public DefenseType currentPlayerDefense;

    public Emotion currentEmotion;
    public StatusBattleActor status = StatusBattleActor.Idle;

    public bool win;
    public bool attendingAttack = false;
    public Transform linkedAttendingAttackPoint;

    int randomizedNumber = 0;

    int index = 0;

    public IEnumerator WinCelebration() {
        win = true;
        SoundManager.instance.PlayVoiceLine(this.self.linkedActor.identifier, "Win");
        yield return new WaitForSeconds(.5f);
    }
    public IEnumerator StartTurn()
    {
        yield return new WaitForSeconds(1f);
    }
    public void ShuffleAnimations() { randomizedNumber = Random.Range(0, 2); animator.SetFloat("Random", (float)randomizedNumber); }
    void Start()
    {
        ogScale = animHandle.localScale;

        if (self.linkedActor.myType == ActorType.Enemy) {
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
        if (this.self.linkedActor.myType == ActorType.Player)
        {
            if (emotion) { result = currentEmotion.ToString() + "_" + animationName; }
            else { result = animationName; }
        }
        else {

            result = animationName;
        }

        if (this.self.linkedActor.myType == ActorType.Enemy) Debug.Log(this.gameObject.name + " Playing -" + result+"-");
        if (animator.IsInTransition(0) || animator.GetCurrentAnimatorStateInfo(0).IsName(result)) return;
        animator.Play(result, 0);
    }
    public void PlayAnimForce(bool emotion, string animationName)
    {


        string result = "";
        if (this.self.linkedActor.myType == ActorType.Player)
        {
            if (emotion) { result = currentEmotion.ToString() + "_" + animationName; }
            else { result = animationName; }
        }
        else
        {

            result = animationName;
        }
        animator.Play(result, 0, 0f);
    }
    public IEnumerator Die()
    {
        this.self.dead = true;
        this.PlayAnim(false, "Dying");
        yield return new WaitForSeconds(0.1f);
        animationInterrupt = true;
        this.PlayAnim(false, "Die");
        SoundManager.instance.PlayVoiceLine(this.self.linkedActor.identifier, "DIE");

        BattleManager.instance.bActors.Remove(self);

        if (this.self.linkedActor.myType == ActorType.Enemy)
        {
            BattleManager.instance.enemyActors.Remove(self);
        }

        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
  
    }
    public IEnumerator Hurt(float force) {
        if (!this.self.dead) { 
            animationInterrupt = true;
            this.PlayAnimForce(false, "Hurt");

            if (this.self.linkedActor.myType == ActorType.Player) SoundManager.instance.PlayVoiceLine(this.self.linkedActor.identifier, "hurt");

            yield return Stun(force);
            animationInterrupt = false;
            if (self.stats.HEALTH.currentValue <= 0 && !this.self.dead) { 
                this.self.dead = true;
                yield return Die(); 
            }
            
        }
    }
    public void SetOnFloor() {
        this.transform.position = new Vector3(this.transform.position.x, BattleManager.instance.assignedBattle.floorY + self.linkedActor.floorYOffset, this.transform.position.z);
    }
    public IEnumerator IA_Goto_Walk(Vector2 position, string endingAnimation) {
        if (!IAMOVING) { 
            if (Vector2.Distance(new Vector3(this.transform.position.x, this.transform.position.z), new Vector3(position.x, position.y)) < 0.6f) {
                IAMOVING = false;
                yield break; 
            }

            while (Vector2.Distance(new Vector3(this.transform.position.x, this.transform.position.z), new Vector3(position.x, position.y)) > 0.6f) {
                this.PlayAnim(true, (Grounded) ? "Walk" : (rb.velocity.y > 0f) ? "Jump" : "Fall");
                IAMOVING = true; 
                this.transform.position = new Vector3(Mathf.MoveTowards(this.transform.position.x, position.x, self.moveSpeed*Time.deltaTime), this.transform.position.y, Mathf.MoveTowards(this.transform.position.z, position.y, self.moveSpeed * Time.deltaTime));
                yield return new WaitForSeconds(0.001f);
            }

            yield return new WaitForSeconds(0.001f);
            
            if (IAMOVING) this.PlayAnim(true, "Idle");
            IAMOVING = false;
        }
    }
    public void UpdateGrounded() {
        Grounded = Physics.Raycast(this.transform.position, Vector3.down, 1.75f, self.linkedActor.floorMask);
        rb.velocity = new Vector3(0f,rb.velocity.y, 0f);

    }

    public IEnumerator Stun(float verticalForce) {
        this.stunned = true;
        int bounces = 5;

        float force = verticalForce;
        rb.velocity = new Vector3(rb.velocity.x, force, rb.velocity.z);
        animationInterrupt = true;
        PlayAnim(false, "Hurt");

        for (int i = 0; i < bounces; i++) {
            while (!Grounded) {
                
                yield return new WaitForSeconds(0.001f);
            }

            force *= 0.85f;
            rb.velocity = new Vector3(rb.velocity.x, force, rb.velocity.z);
            PlayAnim(false, "Hurt");
            yield return new WaitForSeconds(0.01f);
        }
        animationInterrupt = true;
        this.stunned = false;
    }
    public void Jump() {
        if (!canJump) return;
        rb.velocity = new Vector3(rb.velocity.x, self.jumpForce, rb.velocity.z);
        SoundManager.instance.Play("general|jump", false);
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

        if (BattleManager.instance.currentTurn.linkedActor.myType != ActorType.Player)
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
        SoundManager.instance.Play("ui_battle_general|player_hammer_lift", false); // playing the sound
        yield return new WaitForSeconds(0.1f);

        strengthTimer = this.self.getHammerStrength();

        while (self.linkedActor.getKey(KeyEventType.Pressed) && !strengthLoose) {
            yield return new WaitForSeconds(0.1f);
            strengthTimer -= 0.1f;
            if (strengthTimer <= 0F) strengthLoose = true;
        }

        yield return new WaitForSeconds(0.1f);
        this.PlayAnim(false, "HammerReleas" + (string)((strengthLoose) ? "LOSE" : "e"));
        SoundManager.instance.Play("ui_battle_general|player_hammer_release", false); // playing the sound
        hammerlift = false;
        yield return new WaitForSeconds(0.5f);
        animationInterrupt = false;
        strengthLoose = false;
     
    }

 

    public void PrepareForTurn(BattleActorSO character) {

        currentPlayerDefense =  (character.linkedActor.myType == ActorType.Enemy)  ? (DefenseType)(character.getInstance().GetComponent<GenericBActor>().tempAttackID+1) : DefenseType.None;

        status = (this.self == character) ? StatusBattleActor.Think : StatusBattleActor.Defend;

        if (self.linkedActor.myType == ActorType.Player) StartCoroutine(IA_Goto_Walk((this.self != character) ? ((character.linkedActor.myType != ActorType.Player) ? normalPosition : otherPlayerTurnPosition)  : turnPosition, idleAnimation));
    }
    public void UpdateAnimation()
    {
        if (this.self.linkedActor.myType == ActorType.Enemy) return;
            if (win || animationInterrupt) return;

        if (this.self.dead) {
            if (!Grounded)
            {
                this.PlayAnim(false, "Dead_Air");
            }
            else
            {
                this.PlayAnim(false, "Dead_Floor");
            }
            return;
        }

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
    Vector3 ogPos;
    public void AttendAttackUpdate() {
        if (this.self.linkedActor.myType == ActorType.Player) return;
        if (!attendingAttack) return;
        if (linkedAttendingAttackPoint == null) return;
        ogPos = this.transform.position;
        this.transform.localScale = linkedAttendingAttackPoint.transform.localScale;
        this.transform.position = linkedAttendingAttackPoint.transform.position+this.self.linkedPointOffset;
        this.transform.eulerAngles = linkedAttendingAttackPoint.transform.eulerAngles+this.self.linkedAngleOffset;
    }
  
    public void AttendAttackRelease() {
        if (this.self.linkedActor.myType == ActorType.Player) return;
        this.transform.localScale = new Vector3(1f, 1f, 1f);
        this.transform.position = ogPos;
        this.transform.eulerAngles = new Vector3(0f, 0f, 0f);
    }

    public void Update()
    {
        if (this.self.dead) return;

        if (this.self.linkedActor.myType == ActorType.Player && this.win) {
            this.animator.Play("Win_"+BattleManager.instance.getTeamString());
            return;
        }

        AttendAttackUpdate();
        UpdateGrounded();


        if (self.linkedActor.myType == ActorType.Player)
        {
            if (self.linkedActor.getKey(KeyEventType.Pressed) && Grounded)
            {
                if (!BattleManager.instance.inputOverride)
                {

                    if (currentPlayerDefense == DefenseType.Jump || currentPlayerDefense == DefenseType.None) { Jump(); }
                }
            }
            if (currentPlayerDefense == DefenseType.Hammer)
            {
                sidePlayInterrupt = false;
                if (self.linkedActor.getKey(KeyEventType.Pressed) && Grounded)
                {
                    StartCoroutine(Hammer());
                }
            }

        }

        if (!initialized)
        {
            normalPosition = (self.linkedActor.myType == ActorType.Player) ? BattleManager.instance.getPlayerPos(0, self.linkedActor.identifier) : new Vector2(this.transform.position.x, this.transform.position.z);
            turnPosition = (self.linkedActor.myType == ActorType.Player) ? BattleManager.instance.getPlayerPos(1, self.linkedActor.identifier) : new Vector2(this.transform.position.x, this.transform.position.z);
            otherPlayerTurnPosition = normalPosition - new Vector2(1f, 0f);

            initialized = true;
        }
        rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, this.self.maxVerticalSpeed.x, this.self.maxVerticalSpeed.y), rb.velocity.z);

     
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && (self.linkedActor.myType == ActorType.Player)) {
            Battle_Enemy_Attack atk = other.GetComponent<Battle_Enemy_Attack>();
            if (rb.velocity.y >= 0f)
            {
                if (!atk.counterAttack)
                {
                    BattleManagerNumbers.instance.Hurt(2, atk.self, this.self);
                }
            }
            else {
                atk.CounterAttack(this);
            }
        }
    }
    void FixedUpdate()
    {
        if (self.linkedActor.myType == ActorType.Enemy) animator.speed = (MusicManager.instance.myMusic.bpm / 60);

        self.stats.ATTACK.UpdateStat(self.level);
        self.stats.ENERGY.UpdateStat(self.level);
        self.stats.ATTACK.UpdateStat(self.level);
        self.stats.DEFENSE.UpdateStat(self.level);
        self.stats.MAGICATTACK.UpdateStat(self.level);
        self.stats.MAGICDEFENSE.UpdateStat(self.level);
        self.stats.SPEED.UpdateStat(self.level);
        self.stats.LUCK.UpdateStat(self.level);

        if (!animationInterrupt) { 
            UpdateSidePlays();
            UpdateAnimation();
        }
    }

}
