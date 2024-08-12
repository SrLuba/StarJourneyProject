using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHurtMeter : MonoBehaviour
{
    public AudioClip hurtSFX;
    public NumberDisplayer displayer;
    public Animator anim;

    

    public void Init(Transform canvas, int value) {
        SoundManager.instance.Play("battle_general|hurt", false);
        displayer.number = value;
        this.transform.SetParent(canvas);
        anim.Play("PopIn");
       
    }
}
