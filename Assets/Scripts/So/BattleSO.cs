using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ActorSpawnLimitationInformation {
    [Header("Make this random between values below, if this is disabled, just use min as position")] public bool random;
    public Vector2 min, max;


    public Vector2 getResult() {
        return (random) ? new Vector2(Random.Range(min.x,max.x), Random.Range(min.y,max.y)) : min;
    }
}
[System.Serializable]
public class EnemyInformationB {
    public string id;
    
    public int charaCount;

    public ActorSpawnLimitationInformation spawnInfo;

    public CharaSO getChara() {
        CharaSO item = CharaManager.instance.characters.Find(x => x.identifier.ToUpper() == this.id.ToUpper());
        if (item == null) Debug.LogError("Couldn't find CharaSO (" + this.id + ")");
        return item;
    }
}
[System.Serializable]
public enum BattleEnteringCase { 
    Normal,
    Advantage,
    Disadvantage,
    NoTransition,
    DemoNormal,
    DemoAdvantage,
    DemoDisadvantage
}
[CreateAssetMenu]
public class BattleSO : ScriptableObject
{
    public string identifier;
    public string battleName;
    [TextArea] public string battleDescription;
    public MusicSO music;

 


    public BattleEnteringCase enteringCase;
    public float floorY;

    public List<Vector2> playersPositions = new List<Vector2>();
    public List<Vector2> playersPositionsWithTurn = new List<Vector2>();

    [Header("Here goes the enemies")] public List<EnemyInformationB> enemies;
    [Header("Add here allowed players, if you want to make a mario only battle, just just put mario")] public List<string> allowedPlayers;

   
}
