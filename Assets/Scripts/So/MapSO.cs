using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]public class MapSO : ScriptableObject
{
    public AudioClip music;
    public List<MapSection> sections;
    public GameObject Minimap;
}
