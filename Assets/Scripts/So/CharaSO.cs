using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class GenericStat {
    [Header("Starting Value")]public float startValue;
    [Header("Value Multiplier")] public float valueMultiplier;

    public GenericStat(float startValue, float valueMultiplier) {
        this.startValue = startValue;
        this.valueMultiplier = valueMultiplier;
    }
}

[System.Serializable]
public class GenericStats {
    public GenericStat HEALTH;
    public GenericStat ENERGY;
    public GenericStat ATTACK;
    public GenericStat DEFENSE;
    public GenericStat MAGICATTACK;
    public GenericStat MAGICDEFENSE;
    public GenericStat SPEED;
    public GenericStat LUCK;

    public void Save(string path) {
        string selfJSON = JsonUtility.ToJson(this);
        File.WriteAllText(path+".statSave", selfJSON);
    }
    public bool Load(string path)
    {
        if (!File.Exists(path + ".statSave")) return false;

        GenericStats self = JsonUtility.FromJson<GenericStats>(File.ReadAllText(path + ".statSave"));

        this.HEALTH = self.HEALTH;
        this.ENERGY = self.ENERGY;
        this.ATTACK = self.ATTACK;
        this.DEFENSE = self.DEFENSE;
        this.MAGICATTACK = self.MAGICATTACK;
        this.MAGICDEFENSE = self.MAGICDEFENSE;
        this.SPEED = self.SPEED;
        this.LUCK = self.LUCK;
        return true;
    }
}
[System.Serializable]
public class IAInfo {
    [Header("Speed (delta)")]public float speed;
}
public enum CharaType { 
    Player,
    Enemy,
    NPC
}

[System.Serializable]
public class ColliderInformation {
    public float radious;
    public float height;

    public Vector3 offset;

    public ColliderInformation(float radious, float height, Vector3 offset) {
        this.radious = radious;
        this.height = height;
        this.offset = offset;
    }
    public ColliderInformation(float radious, float height)
    {
        this.radious = radious;
        this.height = height;
        this.offset = new Vector3(0f, 0f, 0f);
    }

    public void GetCollider(GameObject g) {
        CapsuleCollider capsule = g.AddComponent<CapsuleCollider>();
        capsule.radius = radious;
        capsule.height = height;
        capsule.center = offset;
    } 
}
[System.Serializable]
public class BattleEntrance_VoiceClips {
    public AudioClip Solo_BattlePrepareVoiceClip;
    public AudioClip Company_BattlePrepareVoiceClip;
    public AudioClip Solo_BattlePrepareVoiceClip_Disadvantage;
    public AudioClip Company_BattlePrepareVoiceClip_Disadvantage;
    public AudioClip Solo_BattlePrepareVoiceClip_Advantage;
    public AudioClip Company_BattlePrepareVoiceClip_Advantage;

    public AudioClip getClip(BattleEnteringCase eCase) {
        bool company = false;
        if (OVManager.instance.playerType == 2) {
            if (!OVManager.instance.secondaryPlayer.split) company = true;
        }

        if (eCase == BattleEnteringCase.Advantage) {
            return (company) ? Company_BattlePrepareVoiceClip_Advantage : Solo_BattlePrepareVoiceClip_Advantage;        
        }
        else if (eCase == BattleEnteringCase.Disadvantage)
        {
            return (company) ? Company_BattlePrepareVoiceClip_Disadvantage : Solo_BattlePrepareVoiceClip_Disadvantage;
        }
        else 
        {
            return (company) ? Company_BattlePrepareVoiceClip : Solo_BattlePrepareVoiceClip;
        }
    } 
}
[CreateAssetMenu]
public class CharaSO : ScriptableObject
{
    [Header("Identifier (ToUpper)")] public string identifier;
    [Header("Name that is Displayed User")] public string displayName;
    [Header("Description that is displayed to User")] [TextArea] public string displayDescription;
    [Header("Character Icon")] public Sprite characterIcon;

    [Header("Character Type")] public CharaType charaType;
    [Header("Character Flags (Depends on Type)")] public List<string> charaFlags;

    [Header("Main Status")] public GenericStats startingStats;

    [Header("Battle")] public BattleActorSO selfBattle;
    [Header("Self PathFinding IA Information")] public IAInfo selfIA;

    [Header("Character Y Offset")] public float floorYOffset;

    [Header("Character Mass")] public float mass = 1f;
    [Header("Character Size")] public float size = 1f;

    [Header("Character Collider")] public ColliderInformation collider;

    [Header("Character Overworld")] public OVActorSO OVActor;

    

    public PhysicMaterial phyMat;

    public float jumpForce = 100f;

    public BattleSO selfEncounter;

    public Vector2 ySquatch, xSquatch;

    public float effectSquatch, effectSquatchY;

    public GameObject selfShadow;
    public Vector3 selfShadowOffset;

    public GameObject IATrailPointPrefab;

    public float followDistance;

    public BattleEntrance_VoiceClips battleEntrance_VoiceClips;

    public string selfLayer;
    public LayerMask floorMask;

    public AudioClip SpeechBubbleSFX;
    public AudioClip SpeechBubbleSkipSFX;
    public AudioClip StompSFX;
    public Transform findSelf() {
        GameObject g = GameObject.Find("OVACTOR_" + identifier.ToUpper());
        return (g!=null) ? g.transform : null;
    }
    public PlayerOV SpawnOnOverworld(Vector3 spawnPoint, PlayerCameraOV camera) {
        GameObject player = new GameObject("OVACTOR_" + identifier.ToUpper());
        player.layer = LayerMask.NameToLayer(selfLayer);


        CapsuleCollider cap = player.AddComponent<CapsuleCollider>();

        cap.height = OVActor.normalHeight;
        cap.radius = OVActor.normalRadius;
        cap.isTrigger = false;
        cap.center = OVActor.normalCenter;

        


        Rigidbody rb = player.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        player.GetComponent<CapsuleCollider>().material = phyMat;

        GameObject Handle = new GameObject("OVACTORHANDLE_" + identifier.ToUpper());
        Handle.transform.position = player.transform.position;
        Handle.transform.SetParent(player.transform);

        GameObject visual = Instantiate(OVActor.Visual, player.transform.position, Quaternion.identity);
        Animator anim = visual.GetComponent<Animator>();

        visual.transform.SetParent(Handle.transform);
        visual.transform.localPosition = OVActor.visualOffset;

        PlayerOV pov = player.AddComponent<PlayerOV>();
        pov.selfChara = this;
        pov.mainCol = cap;
        pov.rb = rb;
        pov.anim = anim;
        pov.handleAngle = Handle.transform;
        pov.rayOffset = new Vector3(0f, 0f, 0f);
        pov.visualHandler = Handle.transform;

        FootController f = visual.GetComponent<FootController>();
        if (f != null) pov.foot = f;
        pov.floorMask = floorMask;
        pov.rayDistance = 1f;

        GameObject shadow = Instantiate(selfShadow, visual.transform.position + selfShadowOffset, Quaternion.identity);
        shadow.AddComponent<ShadowScript>().target = player.transform;
        shadow.GetComponent<ShadowScript>().targetChara = this;
        pov.scriptSh = shadow.GetComponent<ShadowScript>();

        if (OVManager.instance.mainPlayer == null)camera.linkedPlayer = pov;
        GameObject iatp = Instantiate(IATrailPointPrefab, player.transform.position, Quaternion.identity);

        iatp.transform.SetParent(player.transform);

        TrailPoint tp = iatp.AddComponent<TrailPoint>();
        tp.target = pov;
        tp.character = this;

        pov.selfPointTrail = tp;

        pov.Initialize();

        if (OVManager.instance.mainPlayer != null) {
            pov.followPoint = OVManager.instance.mainPlayer.selfPointTrail.transform;
            pov.AI = true;
        }
        if (this.charaType == CharaType.Player) player.tag = "Player";
        if (this.charaType == CharaType.Enemy) player.tag = "Enemy";

        if (this.charaType == CharaType.Enemy) {
            pov.canInput = false;
        }

        OVGenericActor g = player.AddComponent<OVGenericActor>();
        g.selfOV = pov;
        g.anim = anim;

        CapsuleCollider cap2 = player.AddComponent<CapsuleCollider>();
        
        cap2.height = OVActor.ovCollider.height + 0.25f;
        cap2.radius = OVActor.ovCollider.radious + 0.25f;
        cap2.isTrigger = true;
        cap2.center = OVActor.ovCollider.offset;

        player.transform.position = spawnPoint;

        return pov;
    }
}