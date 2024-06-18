using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTargetingManager : MonoBehaviour
{
    public static BattleTargetingManager instance;

    public Sprite UITargetCue;
    public float lerpSpeed = 15f;

    List<GameObject> targets;

    public void Awake()
    {
        instance = this;
    }
    public void RefreshTargets() {
        for (int i = 0; i < targets.Count; i++) {
            targets.Clear();
        }
    }
    void Start()
    {
        
    }
   

    void Update()
    {
        
    }
}
