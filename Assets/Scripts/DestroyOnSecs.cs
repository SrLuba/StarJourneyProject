using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnSecs : MonoBehaviour
{
    public float seconds = 1f;
    void Start()
    {
        Invoke("Dest", seconds);
    }
    void Dest()
    {
        Destroy(this.gameObject);
    }
}
