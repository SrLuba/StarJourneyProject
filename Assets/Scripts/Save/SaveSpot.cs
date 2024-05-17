using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSpot : MonoBehaviour
{

    public AudioClip soundWhenAction;

    public Vector2 ForceValue;

    
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") & other.GetComponent<Rigidbody>().velocity.y < 0f) {
            // Is player and it's falling.
            SoundManager.instance.Play(soundWhenAction);
            other.GetComponent<Rigidbody>().velocity = new Vector3(ForceValue.x, other.GetComponent<Rigidbody>().velocity.y, ForceValue.y);
            SaveManager.instance.ToggleSaveMenu(); // Activating save menu
        }
    }
}
