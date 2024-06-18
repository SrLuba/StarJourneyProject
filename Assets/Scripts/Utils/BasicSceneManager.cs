using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSceneManager : MonoBehaviour
{
    public MusicSO startMusic;
    public void Start()
    {
        if (startMusic != null) MusicManager.instance.PlayClip(startMusic, true);
    }
}
