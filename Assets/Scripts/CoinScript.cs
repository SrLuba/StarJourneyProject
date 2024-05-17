 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    public GameObject particleObj;
    public Vector3 particleOffset;
    public AudioClip grabSound;
    public int value = 1;

    public void Grab() {
        Global.coin += this.value;
        if (particleObj != null)Instantiate(particleObj, this.transform.position + particleOffset, Quaternion.identity);
        if (grabSound != null) SoundManager.instance.Play(grabSound);
        Destroy(this.gameObject);
    }
}
