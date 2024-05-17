using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{

    AudioSource self;
  
    void Start()
    {
        self = this.GetComponent<AudioSource>();
        Invoke("Dest", self.clip.length);
    }

    public void Dest()
    {
        Destroy(this.gameObject);
    }


}
