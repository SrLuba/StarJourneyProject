using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class BattleActorSO : ScriptableObject
{
    [Header("Visual")] public GameObject Visual;
    [Header("Shadow")] public GameObject selfShadow;
    [Header("Shadow Offset")] public Vector3 selfShadowOffset;

    [Header("Visual Offset")] public Vector3 visualOffset, eulerAngleOffset;
    [Header("Animator")] public RuntimeAnimatorController VisualAnimator;

    public ActorSO linkedActor;
    [Header("Main Status")] public GenericStats stats;

    public Vector3 linkedPointOffset;
    public Vector3 linkedAngleOffset;

    public float strength = 2f;

    public List<AttackSO> attackList;

    public int level = 1;
    public ActorSpawnLimitationInformation spawnInfo;
    public float floorY;
    public float moveSpeed = 50f;

    public float jumpForce = 50f;
    public LayerMask MyLayer;
    public LayerMask shadowLayer;
    public PhysicMaterial materialPhy;

    public AudioClip dieVoiceClip;


    public Vector2 maxVerticalSpeed;


    public bool dead = false;

    public int myID = 0;

    public GenericBActor selfInstance;


    public Sprite bgUI;
    public Sprite targetIconUI;


    public GameObject myBattleUI;

    public float stunForce = 100f;

    public float getHammerStrength() {
        return strength;
    }

    public GameObject getInstance() {
        return selfInstance.gameObject;
    }
    public GameObject Spawn(BattleActorSO clone) {
        if (this.linkedActor.myType == ActorType.Enemy) { clone.myID = BattleManager.instance.enemyActors.Count - 1; } else { clone.myID = 0; }
        clone.linkedActor = this.linkedActor;
        
        GameObject player = new GameObject(this.linkedActor.displayName);
    
        GenericBActor bA = player.AddComponent<GenericBActor>();
        bA.self = clone;
        clone.selfInstance = bA;

        Vector2 position = Vector2.zero;

        if (linkedActor.myType == ActorType.Player) { position = BattleManager.instance.getPlayerPos(0, linkedActor.identifier); }
        if (linkedActor.myType != ActorType.Player) { position = spawnInfo.getResult(); }

        player.transform.position = new Vector3(position.x, BattleManager.instance.assignedBattle.floorY + floorY, position.y);

        GameObject visual = Instantiate(Visual, player.transform.position, Quaternion.identity);
        visual.transform.SetParent(player.transform);
        visual.transform.localPosition = visualOffset;
        visual.transform.localEulerAngles = eulerAngleOffset;

        visual.GetComponent<Animator>().runtimeAnimatorController = VisualAnimator;

        bA.animHandle = visual.transform;
        bA.animator = visual.GetComponent<Animator>();

        GameObject shadow = Instantiate(selfShadow, visual.transform.position + selfShadowOffset, Quaternion.identity);
        shadow.AddComponent<ShadowScript>().target = player.transform;
        shadow.GetComponent<ShadowScript>().targetChara = this.linkedActor;
        shadow.GetComponent<ShadowScript>().Offset = selfShadowOffset;
        shadow.GetComponent<ShadowScript>().floorY = visual.transform.position.y;

        shadow.layer = shadowLayer;

        Rigidbody rb = player.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        
        bA.rb = rb;
    

        this.linkedActor.collider.GetCollider(player);
        player.GetComponent<CapsuleCollider>().material = this.materialPhy;

        player.tag = "Player";
        player.layer = MyLayer;


        return player;
    }
}
