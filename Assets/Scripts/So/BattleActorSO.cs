using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BattleActorSO : ScriptableObject
{
    [Header("Visual")] public GameObject Visual;
    [Header("Shadow")] public GameObject selfShadow;
    [Header("Shadow Offset")] public Vector3 selfShadowOffset;

    [Header("Visual Offset")] public Vector3 visualOffset, eulerAngleOffset;
    [Header("Animator")] public RuntimeAnimatorController VisualAnimator;
    
    public ActorSpawnLimitationInformation spawnInfo;
    public float floorY;
    public float moveSpeed = 50f;

    public float jumpForce = 50f;

    public PhysicMaterial materialPhy;

    public List<string> battleIdlesAnimations;
    public string getIdleAnim() {
        return this.battleIdlesAnimations[Random.Range(0, this.battleIdlesAnimations.Count - 1)];
    }
    public GameObject getInstance() {
        GenericBActor[] actores = GameObject.FindObjectsByType<GenericBActor>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
        for (int i = 0; i < actores.Length; i++) {
            if (actores[i].self.selfBattle == this) return actores[i].gameObject; 
        }
        return null;
    }
    public GameObject Spawn(CharaSO character) {
        GameObject player = new GameObject(character.name);
        
        GenericBActor bA = player.AddComponent<GenericBActor>();
        bA.self = character;

        Vector2 position = Vector2.zero;

        if (character.charaType == CharaType.Player) { position = BattleManager.instance.getPlayerPos(0, character.identifier); }
        if (character.charaType != CharaType.Player) { position = spawnInfo.getResult(); }

        player.transform.position = new Vector3(position.x, BattleManager.instance.assignedBattle.floorY + character.selfBattle.floorY, position.y);

        GameObject visual = Instantiate(Visual, player.transform.position, Quaternion.identity);
        visual.transform.SetParent(player.transform);
        visual.transform.localPosition = visualOffset;
        visual.transform.localEulerAngles = eulerAngleOffset;

        visual.GetComponent<Animator>().runtimeAnimatorController = VisualAnimator;

        bA.animHandle = visual.transform;
        bA.animator = visual.GetComponent<Animator>();

        GameObject shadow = Instantiate(selfShadow, visual.transform.position + selfShadowOffset, Quaternion.identity);
        shadow.AddComponent<ShadowScript>().target = player.transform;
        shadow.GetComponent<ShadowScript>().targetChara = character;
        shadow.GetComponent<ShadowScript>().Offset = selfShadowOffset;
        shadow.GetComponent<ShadowScript>().floorY = visual.transform.position.y;

        Rigidbody rb = player.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        
        bA.rb = rb;


        character.collider.GetCollider(player);
        player.GetComponent<CapsuleCollider>().material = this.materialPhy;

        player.tag = "Player";

        return player;
    }
}