using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ActorOV : MonoBehaviour
{
    [HideInInspector] public ActorSO selfActor;

    public ActorType myType;

    bool initialized = false;

    public Animator anim;
    public bool split = false;
    public bool AI = false;
    public bool canInput = false;
    public bool Grounded = false;

    public Vector2 input;
    public Vector2 animV;
    public void Log(int type, string txt) {
        switch (type)
        {
            case 0:
                Debug.Log("<color=green>  ActorOV - "+this.gameObject.name+" > "+txt+"</color>");
                break;
            case 1:
                Debug.Log("<color=yellow> ActorOV - " + this.gameObject.name + " > " + txt + "</color>");
                break;
            case 2:
                Debug.Log("<color=red> ActorOV - " + this.gameObject.name + " > " + txt + "</color>");
                break;
        }
    }
   
    public void Initialize() {
        this.Log(0, "Initializing!");
        this.myType = this.selfActor.myType;

        this.initialized = true;
    }
    void Start()
    {
        if (!this.initialized) this.Initialize();
    }

    
    void Update()
    {
        
    }
}
