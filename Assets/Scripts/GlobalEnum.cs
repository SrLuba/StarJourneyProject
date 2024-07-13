using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GenericStat
{
    [Header("Starting Value")] public int startValue;
    [Header("Value Multiplier")] public int valueMultiplier;
    [Header("Value Add")] public int addValue;
    [Header("Current Value")] public int currentValue;
    [Header("Max Value")] public int maxValue;

    public GenericStat(int startValue, int valueMultiplier, int addValue)
    {
        this.startValue = startValue;
        this.valueMultiplier = valueMultiplier;
        this.addValue = addValue;
        this.currentValue = startValue;
    }

    public void UpdateStat(int level)
    {
        this.maxValue = startValue * (valueMultiplier * level) + addValue;
    }
    public void ResetStat(int level)
    {
        this.UpdateStat(level);
        this.currentValue = this.maxValue;
        this.valueMultiplier = 1;
    }
}

[System.Serializable]
public class GenericStats
{
    public GenericStat HEALTH;
    public GenericStat ENERGY;
    public GenericStat ATTACK;
    public GenericStat DEFENSE;
    public GenericStat MAGICATTACK;
    public GenericStat MAGICDEFENSE;
    public GenericStat SPEED;
    public GenericStat LUCK;
}
[System.Serializable]
public class ColliderInformation
{
    public float radious;
    public float height;

    public Vector3 offset;

    public ColliderInformation(float radious, float height, Vector3 offset)
    {
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

    public void GetCollider(GameObject g)
    {
        CapsuleCollider capsule = g.AddComponent<CapsuleCollider>();
        capsule.radius = radious;
        capsule.height = height;
        capsule.center = offset;
    }
}
[System.Serializable]
public enum ActorType
{
    Generic,
    NPC,
    Player,
    Enemy
}
public enum ActionOwner
{
    A,
    B,
    Y,
    X
}

public enum KeyEventType
{
    Pressed,
    Holding,
    Released
}

[System.Serializable]
public class BattleEntrance {
    public List<string> characters;
    public MusicSO musicSO;
}

[System.Serializable]public class BattleEntranceList
{
    public List<BattleEntrance> info;

    public List<string> test;

    public BattleEntrance get() {
  
        List<string> currentCharacters = new List<string>(StaticManager.instance.game.getPlayersIDS());
        test = new List<string>(StaticManager.instance.game.getPlayersIDS());
        for (int i = 0; i < info.Count; i++)
        {
            bool has = true;
            for (int a = 0; a < currentCharacters.Count; a++){
                if (!info[i].characters.Contains(currentCharacters[a])) has = false;
            }

            if (has) return info[i];
        }

        return null;
    }
}