using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ranking {
    public string name = "OK";
    public string soundExpression;
    public GameObject Prefab;
    public GameObject PrefabBG;
    public Sprite assignedSprite;
    public Vector3 offset;
}
[CreateAssetMenu]
public class RankingSO : ScriptableObject
{
    public List<Ranking> rankings;
}
