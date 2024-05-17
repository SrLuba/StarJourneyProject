using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class MusicSO : ScriptableObject
{
    [Header("Music Loop")] public AudioClip musicLoop;
    [Header("Music Start (Intro)")] public AudioClip musicStart;

    public float musicOffset;
}
