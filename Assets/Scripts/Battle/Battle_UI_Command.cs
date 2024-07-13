using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Battle_UI_Command : MonoBehaviour
{
    public string playerID;

     ActorSO actor;
    public Image i_bg, i_key;

    public Animator animator;


    public List<Sprite> temporalKeyImages;

    public bool active = true;
    public string state = "Jump";

    public List<Vector2> position;


    public void Update()
    {
        if (actor == null) {
            this.actor = Global.FindActorByID(playerID);
        }
        if (actor == null) this.enabled = false;

        this.transform.localPosition = new Vector3(position[StaticManager.instance.PlayerCount - 1].x, position[StaticManager.instance.PlayerCount - 1].y, this.transform.localPosition.z);

        i_bg.sprite = actor.selfBattle.bgUI;
        i_key.sprite = temporalKeyImages[actor.currentPlayerID];

        i_bg.SetNativeSize();
        i_key.SetNativeSize();
        AnimatorUpdate();
    }

    public void AnimatorUpdate() {
        if (!active) {
            animator.Play("IDLE");
            return;
        }

        animator.Play(this.state);
    }

}
