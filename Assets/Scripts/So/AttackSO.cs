using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class AttackSOItem {
    public string linkedActor;
    public Vector3 offset;
    public Vector2 positionOffset;
    public GameObject attack;
}
[CreateAssetMenu]
public class AttackSO : ScriptableObject
{
    public List<AttackSOItem> attacks;

    public AttackSOItem getAttack(BattleActorSO actor) {
        return attacks.Find(x => x.linkedActor.ToUpper() == actor.linkedActor.identifier.ToUpper());
    }
}
