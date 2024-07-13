using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;



    public Battle_Camera camera;

    public void BattleUpdate() {
        if (camera == null) { 
            camera = GameObject.FindAnyObjectByType<Battle_Camera>();   
        }
    }
    public void Awake()
    {
        instance = this;
    }
}
