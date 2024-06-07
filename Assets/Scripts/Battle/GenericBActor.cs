using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    void Start()
    {
        ogScale = animHandle.localScale;

      
    }

    public void SetOnFloor() {
        this.transform.position = new Vector3(this.transform.position.x, BattleManager.instance.assignedBattle.floorY + self.floorYOffset, this.transform.position.z);
    }
    public IEnumerator IA_Goto_Walk(Vector2 position, string endingAnimation) {
        animationState = "Walk";

        while (Vector3.Distance(new Vector3(this.transform.position.x, 0f, this.transform.position.z), new Vector3(position.x, 0f, position.y)) > 0.1f) {
            IAMOVING = true;
            this.transform.position = Vector3.MoveTowards(this.transform.position, new Vector3(position.x, this.transform.position.y, position.y), self.selfBattle.moveSpeed*Time.deltaTime);
            Debug.Log("<color=orange>pos " + this.transform.position.x.ToString() + "|" + this.transform.position.y.ToString() + "|" + this.transform.position.z.ToString() + "||| dest |" + position.x.ToString() + "|" + this.transform.position.y.ToString() + "|" + position.y + "</color>");
            yield return new WaitForSeconds(0.001f);
        }

        yield return new WaitForSeconds(0.001f);
        IAMOVING = false;
        animationState = endingAnimation;
    }

    public void Update()
    {
        if (self.charaType == CharaType.Player) {
            if (self.getActionPress()) {
                rb.velocity = new Vector3(rb.velocity.x,self.jumpForce, rb.velocity.z);
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
        idleAnimation = (this.self == character) ? self.selfBattle.getIdleAnim() : "Idle";
        StartCoroutine(IA_Goto_Walk((this.self != character) ? ((character.charaType != CharaType.Player) ? normalPosition : otherPlayerTurnPosition)  : turnPosition, idleAnimation));

    }

    void FixedUpdate()
    {
        animator.Play(animationState);
        if (IAMOVING) { animationState = "Walk"; } else {
            animationState = idleAnimation;
        }
        animHandle.localScale = Vector3.Lerp(animHandle.localScale, ogScale + new Vector3(Mathf.Clamp(rb.velocity.y * self.effectSquatch, self.xSquatch.x, self.xSquatch.y), Mathf.Clamp(rb.velocity.y* self.effectSquatchY, self.ySquatch.x, self.ySquatch.y), Mathf.Clamp(rb.velocity.y * self.effectSquatch, self.xSquatch.x, self.xSquatch.y)), 15f*Time.deltaTime);
    }
}
