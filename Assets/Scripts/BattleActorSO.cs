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

    public GameObject Spawn(CharaSO character) {
        GameObject player = new GameObject(character.name);
        
        GenericBActor bA = player.AddComponent<GenericBActor>();
        bA.self = character;
        

        Vector2 position = spawnInfo.getResult();
        player.transform.position = new Vector3(position.x, BattleManager.instance.assignedBattle.floorY + character.floorYOffset, position.y);

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

        return player;
    }
}
