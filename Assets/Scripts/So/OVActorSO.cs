using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum OP_UseRequirement { 
    Always,
    Grounded,
    Air
}
/*[System.Serializable]
public class OverworldPerk {
    public string perkName;
    public string animationName;
    public float duration;

    public Sprite icon; 

    public AudioClip audioClip;
    public AudioClip audioClip2;

    public GameObject FX;
    public Vector3 FXOffset;


    public AudioClip selfClipWhenSelect;
    public AudioClip adjacentClipWhenSelect;

    public GameObject actionCollider;
    public float actionColliderDistance;
    public float actionColliderDelay;
    public float actionColliderDuration;

    public string argument = "";

    public OP_UseRequirement useRequeriment = OP_UseRequirement.Grounded;


    public IEnumerator Trigger(PlayerOV player) {
        OVActionManager.instance.busyPerk = true;

        Debug.Log("PERK | " + perkName);

        if (argument.ToUpper() == "WATERIN") 
        {
            player.underwaterControl = true;
            if (OVManager.instance.secondaryPlayer != null) OVManager.instance.secondaryPlayer.underwaterControl = true;
        }
        else if (argument.ToUpper() == "WATEROUT")
        {
            player.underwaterControl = false;
            if (OVManager.instance.secondaryPlayer != null) OVManager.instance.secondaryPlayer.underwaterControl = false;
        }

        player.canAnimate = false;
        player.anim.Play(this.animationName);
        Debug.Log("<color=yellow>Perk Triggered | " + this.perkName+ "</color>");
        player.SetupInterrupt(false, player.animV);
        player.input = new Vector2(0f, 0f);
        SoundManager.instance.Play(audioClip);

        yield return new WaitForSeconds(actionColliderDelay);

        GameObject FXR = MonoBehaviour.Instantiate(FX, player.transform.position + (player.handleAngle.forward * actionColliderDistance) + FXOffset, Quaternion.identity);
        FXR.transform.eulerAngles = new Vector3(-90f, 0f, 0f);
        GameObject actionCol = MonoBehaviour.Instantiate(actionCollider, player.transform.position + new Vector3(player.animV.x * actionColliderDistance, 0f, player.animV.y * actionColliderDistance), Quaternion.identity);
        
        yield return new WaitForSeconds(actionColliderDuration);
        
        SoundManager.instance.Play(audioClip2);
        MonoBehaviour.Destroy(actionCol);
        
        yield return new WaitForSeconds(duration);
        player.SetupInterrupt(true, player.animV);
        player.input = new Vector2(0f, 0f);
        
        OVActionManager.instance.busyPerk = false;
        if (!StaticManager.instance.onBattle)
        { player.anim.Play("Idle"); player.canAnimate = true; Debug.Log("<color=red>Setting Can Animate To True</color>"); }
    }
}*/
[System.Serializable]
public class PerksPack {
    public List<PerkSO> perks;
}
[CreateAssetMenu]
public class OVActorSO : ScriptableObject
{
    public float speed;
    public float jumpForce;

    public float underWaterYOffset = 2f;

    public float acceleration = 15f;
    public float deceleration = 25f;
    public float angleEffect = 15f;

    public float gravityJump = 15f;
    public float gravityNormal = 1f;

    public float gravityJump_Water = 15f;
    public float gravityNormal_Water = 1f;
    public float acceleration_Water = 15f;
    public float deceleration_Water = 25f;
    public float jumpForce_Water = 10f;
    public float speed_Water = 10f;

    public float animationSpeedMax = 2f;
    public float AIFollowDistance;

    public GameObject inputCuePrefab;
    public GameObject splashFX;


    public float animationSpeedMultiplier = 1f;

    public bool canUseHammer = true;
    [HideInInspector]public bool hammerUnlocked = true;

    public List<PerksPack> perks;

    public ColliderInformation ovCollider;

    public AudioClip jumpSFX;

    public GameObject Visual;
    public Vector3 visualOffset;
    public RuntimeAnimatorController animController;

    public float normalRadius, normalHeight;
    public Vector3 normalCenter;

    public float squishRadius, squishHeight;
    public Vector3 squishCenter;

    public Vector2 yVelClamp = new Vector2(-50f, 999f);
    public Vector2 yVelClampWater = new Vector2(-50f, 999f);
}
