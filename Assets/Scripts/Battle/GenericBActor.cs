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

    void Start()
    {
        ogScale = animHandle.localScale;
    }

    public void SetOnFloor() {
        this.transform.position = new Vector3(this.transform.position.x, BattleManager.instance.assignedBattle.floorY + self.floorYOffset, this.transform.position.z);
    }
    public void IA_Goto_Walk() { }

    public void Update()
    {
        if (self.charaType == CharaType.Player) {
            if (Keyboard.current.FindKeyOnCurrentKeyboardLayout(self.charaFlags[0]).wasPressedThisFrame) {
                rb.velocity = new Vector3(rb.velocity.x,self.jumpForce, rb.velocity.z);
            }
        }
    }

    void FixedUpdate()
    {
        animHandle.localScale = Vector3.Lerp(animHandle.localScale, ogScale + new Vector3(Mathf.Clamp(rb.velocity.y * self.effectSquatch, self.xSquatch.x, self.xSquatch.y), Mathf.Clamp(rb.velocity.y* self.effectSquatchY, self.ySquatch.x, self.ySquatch.y), Mathf.Clamp(rb.velocity.y * self.effectSquatch, self.xSquatch.x, self.xSquatch.y)), 15f*Time.deltaTime);
    }
}
