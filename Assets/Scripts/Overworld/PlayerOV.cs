using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerOV : MonoBehaviour
{
    public bool AI = false;
    public bool followTransform = true;

    public Transform followPoint;
    public Vector3 followVector;

    public TrailPoint selfPointTrail;

    public CharaSO selfChara;

    public Rigidbody rb;
    public Animator anim;


    public Transform handleAngle;

    public bool Grounded = true;

    public Vector3 rayOffset;
    public float rayDistance;

    public FootController foot;

    public float angleOffset;

    public ShadowScript scriptSh;

    public Vector2 input = new Vector2(0f, 0f);
    public Vector2 animV = new Vector2(0f, 0f);
    public bool canPhysics = true;
    public bool p2 = false;

    public bool canInput = true;

    public bool canAnimate = true;

    public bool moving;

    public bool animationOverride = false;

    public bool joystick = false;

    public Vector3 angle;

    public LayerMask floorMask;
    Transform child;
    public bool split;
    string attackProgress = "";

    public void PrepareForBattle(bool attacked) {
        canAnimate = false;
        if (attacked) anim.Play("AttackSuccess_" + attackProgress);
        canPhysics = false;
        canInput = false;
    }

    public void Start()
    {
        canPhysics = true;
        if (OVManager.instance.mainPlayer != this && OVManager.instance.p2Enable)
        {
            p2 = true;
            this.AI = false;
        }
        this.split = false;
        child = new GameObject("t").transform;
    }

    public void Initialize()
    {
        this.canInput = true;
        this.split = false;
    }
   
    public void StopMoving() {
        this.input = new Vector2(0f, 0f);
    }

    public IEnumerator AnimationOverrideDisable() {
        yield return new WaitForSeconds(1f);
        while (this.anim.GetCurrentAnimatorStateInfo(0).IsName("SquishPerk_Squish") || this.anim.GetCurrentAnimatorStateInfo(0).IsName("SquishPerk_Unsquish"))
        {
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.01f);
        this.anim.Play("Idle");
        StopMoving();
        animationOverride = false;
        canInput = true;
    }

    public bool squished = false;
    public CapsuleCollider mainCol;
    public void SquishToggle() {
        animationOverride = true;
        
        squished = !squished;
        this.anim.Play("SquishPerk_" + (squished ? "Squish" : "Unsquish"));
        canInput = false;
        StopMoving();
        StartCoroutine(AnimationOverrideDisable());
    }

    public void DoPerk(int id) {
        if (OVActionManager.instance == null) return;
        if (OVActionManager.instance.busyPerk) return;

        StartCoroutine(selfChara.OVActor.perks[id].Trigger(this));
        attackProgress = selfChara.OVActor.perks[id].perkName;

        if (StaticManager.instance.company && OVManager.instance.secondaryPlayer == this && id == 1) {
            if (!OVManager.instance.secondaryPlayer.split) {
                if (!EventManager.instance.onEvent) { OVManager.instance.mainPlayer.SquishToggle(); } else {
                    if (OVManager.instance.mainPlayer.squished) OVManager.instance.mainPlayer.SquishToggle();
                    OVManager.instance.mainPlayer.squished = false;
                }
            }
        }
    }
    public void UpdateGrounded() {
        RaycastHit hit;

        if (Physics.Raycast(this.transform.position + rayOffset, Vector3.down, out hit, Mathf.Infinity, floorMask)) {
            scriptSh.floorY = hit.point.y + 0.001f;
            Vector3 ta = Quaternion.FromToRotation(Vector3.up, hit.normal).eulerAngles;
            
            angle = new Vector3(ta.x * (float)((((Mathf.Abs(angle.y) >= 135f && Mathf.Abs(angle.y) < 270f)) ? -1f : 1f)), angle.y, ta.z * (((Mathf.Abs(angle.y) >= 89f && Mathf.Abs(angle.y) < 91f) || (Mathf.Abs(angle.y) >= 269f && Mathf.Abs(angle.y) < 281f)) ? 0f : ((Mathf.Abs(angle.y) >= 89f && Mathf.Abs(angle.y) <= 91f) || (Mathf.Abs(angle.y) >= 135f && Mathf.Abs(angle.y) < 270f)) ? -1f : 1f));

        }

        if (Physics.Raycast(this.transform.position + rayOffset, Vector3.down, out hit, rayDistance, floorMask))
        {
            if (foot!=null)foot.floorType = hit.collider.gameObject.name.Split('|')[0];
            Grounded = true;
        }
        else
        {
            Grounded = false;
        }
    }
    public void Jump() {
        if (!Grounded) return;
        attackProgress = "Jump";
        SoundManager.instance.Play(selfChara.OVActor.jumpSFX);
        rb.velocity = new Vector3(rb.velocity.x, selfChara.OVActor.jumpForce, rb.velocity.y);
    }

    public void DoAction() {
        if (!Grounded) return;
        if (EventManager.instance.onEvent) return;

        bool adjacent = OVManager.instance.secondaryPlayer == this;
        if (!adjacent)
        {
            if (OVActionManager.instance.actionID_Player != 0)
            {

                DoPerk(OVActionManager.instance.actionID_Player);
                return;
            }
        }
        else {
            if (OVActionManager.instance.actionID_Adjacent != 0)
            {

                if (!OVManager.instance.secondaryPlayer.split) DoPerk(OVActionManager.instance.actionID_Adjacent);
                return;
            }
        }

        Jump();
    }
    public void OnTriggerExit(Collider other) {
        if (other.CompareTag("SplitZone"))
        {
            if (OVManager.instance.playerType != 2) return;

            if (this == OVManager.instance.secondaryPlayer)
            {
                OVManager.instance.mainPlayer.split = false;
            }
            else
            {
                OVManager.instance.secondaryPlayer.split = false;
            }
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Container") && this.selfChara.charaType == CharaType.Player && this.rb.velocity.y > 0f) {
            other.GetComponent<BlockContainer>().Hit((OVManager.instance.mainPlayer == this) ? 0 : 1);
            this.rb.velocity = new Vector3(0f, 0f, 0f);
        }
        if (other.CompareTag("Coin") && this.selfChara.charaType == CharaType.Player)
        {
            other.GetComponent<CoinScript>().Grab();
        }

        if (EventManager.instance.onEvent) return;
        if (other.CompareTag("SplitZone") && this.selfChara.charaType == CharaType.Player) {
            if (OVManager.instance.playerType != 2) return;

            if (this == OVManager.instance.secondaryPlayer)
            {
                OVManager.instance.mainPlayer.split = true;
            }
            else {
                OVManager.instance.secondaryPlayer.split = true;
            }
            
        }
       
        BattleEnteringCase eCase = BattleEnteringCase.Normal;
        if (other.CompareTag("Hammer_0") && this.selfChara.charaType == CharaType.Enemy)
        {
            Debug.Log("<color=blue>HAMMER</color>");
            if (StaticManager.instance.onBattle) return;

            PlayerOV a = OVManager.instance.mainPlayer;
            a.canInput = false;
            a.canAnimate = false;
            a.rb.velocity = new Vector3(0f, 0f, 0f);
            a.rb.isKinematic = true;
            SoundManager.instance.Play(a.selfChara.OVActor.perks[1].audioClip2);

            eCase = BattleEnteringCase.Advantage;
            
            StaticManager.instance.onBattle = true;

            

            a.PrepareForBattle(true);

            this.canAnimate = false;
            this.PrepareForBattle(false);
            this.anim.Play("BDisadvantage");

            if (OVManager.instance.playerType == 2)
            {
                if (!OVManager.instance.secondaryPlayer.split)
                {
                    OVManager.instance.secondaryPlayer.PrepareForBattle(false);
                    OVManager.instance.secondaryPlayer.anim.Play("AttackSuccess_Hammer_SidePlay");
                }
            }

            StaticManager.instance.battleAdvantageCase = 2;

            StartCoroutine(TransitionManager.instance.StartTransitionBattle(eCase, a == OVManager.instance.mainPlayer, this.selfChara.selfEncounter));
        }



        if (animationOverride) return;
        if (other.CompareTag("Enemy") && this.selfChara.charaType == CharaType.Player)
        {
            if (StaticManager.instance.onBattle) return;
            canInput = false;
            canAnimate = false;
            rb.velocity = new Vector3(0f, 0f, 0f);
            rb.isKinematic = true;

            StaticManager.instance.onBattle = true;

            PlayerOV enemy = other.GetComponent<PlayerOV>();
            this.canAnimate = false;


            if (this.transform.position.y > enemy.transform.position.y + 0.2f)
            {
                SoundManager.instance.Play(this.selfChara.StompSFX);
                enemy.canAnimate = false;
                enemy.PrepareForBattle(false);
                enemy.anim.Play("BDisadvantage");

                eCase = BattleEnteringCase.Advantage;

                if (OVManager.instance.playerType == 2)
                {
                    if (!OVManager.instance.secondaryPlayer.split) { 
                        OVManager.instance.secondaryPlayer.PrepareForBattle(true);
                        OVManager.instance.secondaryPlayer.anim.Play("AttackSuccess_Stomp_SidePlay");
                    }
                }
                else
                {
                    OVManager.instance.mainPlayer.PrepareForBattle(true);
                    OVManager.instance.mainPlayer.anim.Play("AttackSuccess_Stomp_SidePlay");
                }

                StaticManager.instance.battleAdvantageCase = 1;

                BattleSO so = enemy.selfChara.selfEncounter;
                StartCoroutine(TransitionManager.instance.StartTransitionBattle(eCase, this == OVManager.instance.mainPlayer, so));
                return;
         
            }
            else
            {
                StaticManager.instance.battleAdvantageCase = 0;
                if ((animV.x < 0f && enemy.animV.x < 0f) || (animV.x > 0f && enemy.animV.x > 0f) || (animV.x < 0f && enemy.animV.x < 0f) || (animV.y > 0f && enemy.animV.y > 0f) || (animV.y < 0f && enemy.animV.y < 0f)) {
                    eCase = BattleEnteringCase.Disadvantage;
                    this.anim.Play("BattleEntrance_Disadvantage");
                    enemy.anim.Play("BAdvantage");
                    if (OVManager.instance.playerType == 2)
                    {
                        if (this == OVManager.instance.mainPlayer)
                        {
                            if (!OVManager.instance.secondaryPlayer.split)
                            {
                                OVManager.instance.secondaryPlayer.PrepareForBattle(true);
                                OVManager.instance.secondaryPlayer.anim.Play("BattleEntrance_Disadvantage_SidePlay");
                                OVManager.instance.secondaryPlayer.canInput = false;
                                OVManager.instance.secondaryPlayer.animV = this.animV;
                            }
                        }
                        else
                        {
                            OVManager.instance.mainPlayer.PrepareForBattle(true);
                            OVManager.instance.mainPlayer.anim.Play("BattleEntrance_Disadvantage_SidePlay");
                            OVManager.instance.mainPlayer.canInput = false;
                            OVManager.instance.mainPlayer.animV = this.animV;
                        }
                    }
                }
                else
                {
                    enemy.anim.Play("BNormal");
                    OVManager.instance.mainPlayer.PrepareForBattle(false);
                    OVManager.instance.mainPlayer.anim.Play("AttackSuccess");
                    if (OVManager.instance.playerType == 2)
                    {
                        if (OVManager.instance.secondaryPlayer != null)
                        {
                            if (!OVManager.instance.secondaryPlayer.split)
                            {
                                OVManager.instance.secondaryPlayer.PrepareForBattle(false);
                                OVManager.instance.secondaryPlayer.anim.Play("AttackSuccess");
                            }
                        }
                    }
                    eCase = BattleEnteringCase.Normal;
                }
                BattleSO so = enemy.selfChara.selfEncounter;
                StartCoroutine(TransitionManager.instance.StartTransitionBattle(eCase, this == OVManager.instance.mainPlayer, so));
                return;
            }

       
        

        }

        if (OVManager.instance.mainPlayer != this) return;

        if (other.CompareTag("Warp")) {
            OVManager.instance.Warp(other.name.Split('|')[0].ToUpper(), other.name.Split('|')[1].ToUpper());
        }
    }
    public void SetupInterrupt(bool ovInput, Vector2 direction) {
        input = direction;
        animV = direction;
        canInput = ovInput;
    }
    public void SetupInterrupt(Vector2 direction)
    {
        input = direction;
        animV = direction;
    }
    public void UpdateAI() {

        if (StaticManager.instance.onBattle) {
            input = new Vector2(0f, 0f);
            return;
        }
        if (followPoint == null && followTransform) return;
        if (split) {
            input = new Vector2(0f, 0f);

            return;
        }

        Vector3 tPos = followTransform ? followPoint.position : followVector;

        float ix = (this.transform.position.x > tPos.x + 0.4f) ? -1f : (this.transform.position.x < tPos.x - 0.4f) ? 1f : 0f;
        float iy = (this.transform.position.z > tPos.z + 0.4f) ? -1f : (this.transform.position.z < tPos.z - 0.4f) ? 1f : 0f;

        if (Vector3.Distance(this.transform.position, tPos) < 0.4f)
        {
            ix = 0f;
            iy = 0f;
        }
       
        input = new Vector2(ix, iy);
        if (ix != 0f || iy != 0f) animV = new Vector2(ix, iy);
    }
    public void UpdateInput() {
        if (EventManager.instance.onEvent) return;
        if (StaticManager.instance.onBattle)
        {
            input = new Vector2(0f, 0f);
            return;
        }

        if (OVManager.instance.playerType == 2)
        {
            if (OVManager.instance.mainPlayer != this)
            {
                if (InputManager.instance.bPress) DoAction();
            }
            else
            {
                if (InputManager.instance.aPress) DoAction();
            }
        }
        else {
            if (InputManager.instance.aPress) DoAction();
        }
           
        if (AI)
        { canInput = false; return; }
        if (!canInput) return;
        float iX = InputManager.instance.leftStick.x;
        float iY = InputManager.instance.leftStick.y;
        input = new Vector2(iX, iY);

        if (iX != 0f || iY != 0f) animV = new Vector2(iX, iY);        
    }
    
    public void UpdateAnim() {
        
        moving = input.magnitude > 0.1f;

        if (animationOverride) return;
   
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(animV.x, 0f, animV.y));
           if (this.selfChara.charaType == CharaType.Player) angle = new Vector3(angle.x, (!split) ? (newRotation.eulerAngles.y + angleOffset) : 180f + angleOffset, angle.z);
   
         if (canAnimate)
        {
        
            anim.Play((Grounded) ? (moving) ? "Move" : (split) ? "Idle_Split" : "Idle" : (rb.velocity.y > 0f) ? "Jump" : "Fall");
        }
    }

    public void UpdatePhysics() {
        if (rb == null) return;
        if (!canPhysics) {
            rb.velocity = new Vector3(0f, 0f, 0f);
            return;
        }
     
         rb.velocity = new Vector3(selfChara.OVActor.speed * input.normalized.x, Mathf.Clamp(rb.velocity.y, selfChara.OVActor.yVelClamp.x, selfChara.OVActor.yVelClamp.y), selfChara.OVActor.speed * input.normalized.y);
     
    }
    public void FixedUpdate()
    {
        UpdatePhysics();
        UpdateAnim();
    }
    bool noticeInterrupt = false;
    public IEnumerator Notice() {
        noticed = false;
        canAnimate = false;
        canInput = false;
        animationOverride = true;
        noticeInterrupt = true;

        SoundManager.instance.Play(selfChara.noticeSFX);
        anim.Play("Notice");
        yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0)[0].clip.length);

        noticed = true;
        canAnimate = true;
        canInput = true;
        animationOverride = true;
        noticeInterrupt = false;
    }
    bool noticed = false;

    public void EnemyUpdate()
    {
        if (EventManager.instance.onEvent) return;
        if (StaticManager.instance.onBattle)
        {
            input = new Vector2(0f, 0f);
            return;
        }
        Transform targetPlayer = OVManager.instance.mainPlayer.transform;

        float ix = 0f;
        float iy = 0f;

        if (noticed)
        {
            ix = (this.transform.position.x > targetPlayer.position.x + 0.1f) ? -1f : (this.transform.position.x < targetPlayer.position.x - 0.1f) ? 1f : 0f;
            iy = (this.transform.position.z > targetPlayer.position.z + 0.1f) ? -1f : (this.transform.position.z < targetPlayer.position.z - 0.1f) ? 1f : 0f;
            
            float g = Quaternion.FromToRotation(this.transform.forward, OVManager.instance.mainPlayer.transform.position).eulerAngles.y;
            angle = new Vector3(angle.x, g+180f, angle.z);
        }
        else {
            ix = 0f;
            iy = 0f;
            if (!noticeInterrupt)
            {
                if (Vector3.Distance(this.transform.position, targetPlayer.position) < selfChara.noticeDistance)
                {
                    StartCoroutine(Notice());
                }
            }
            else {
                ix = 0f;
                iy = 0f;
            }
        }

        input = new Vector2(ix, iy);
        if (ix != 0f || iy != 0f) animV = new Vector2(ix, iy);
    }
    public Transform visualHandler;
    public void Update()
    {

        mainCol.height = (squished) ? selfChara.OVActor.squishHeight : selfChara.OVActor.normalHeight;
        mainCol.radius = (squished) ? selfChara.OVActor.squishRadius : selfChara.OVActor.normalRadius;
        mainCol.center = (squished) ? selfChara.OVActor.squishCenter : selfChara.OVActor.normalCenter;
        visualHandler.localScale = (squished) ? new Vector3(1f, 0.5f, 1f) : new Vector3(1f, 1f, 1f);


        if (this.AI) UpdateAI();
        if (selfChara.charaType == CharaType.Player) UpdateInput();
        UpdateGrounded();
        if (EventManager.instance.onEvent) return;
        if (selfChara.charaType == CharaType.Enemy) EnemyUpdate();
        handleAngle.transform.eulerAngles = new Vector3(Mathf.LerpAngle(handleAngle.transform.eulerAngles.x, angle.x,15f*Time.deltaTime), Mathf.LerpAngle(handleAngle.transform.eulerAngles.y, angle.y, 15f * Time.deltaTime), Mathf.LerpAngle(handleAngle.transform.eulerAngles.z, angle.z, 15f * Time.deltaTime));
    }

}
