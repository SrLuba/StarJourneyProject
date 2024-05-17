using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public AudioClip BreakClip;
    public GameObject FX;
    public Vector3 FXOffset;
    public string breakConditionTag = "Hammer_0";

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(breakConditionTag)) {
            SoundManager.instance.Play(BreakClip);
            Instantiate(FX, this.transform.position + FXOffset, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
