using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ranking {
    public string name = "OK";
    public AudioClip sound;
    public GameObject Prefab;
    public GameObject PrefabBG;
}
[CreateAssetMenu]
public class RankingSO : ScriptableObject
{
    public List<Ranking> rankings;
}