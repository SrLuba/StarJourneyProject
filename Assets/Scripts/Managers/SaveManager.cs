using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    public void Awake()
    {

        if (instance != null) {
            Destroy(instance.gameObject);
        }
        instance = this;
        
    }
    public IEnumerator SaveMenuOnSequence() {
        yield return new WaitForSeconds(1f); // wait 1 second
    }
    public void ToggleSaveMenu() {
        GenericBActor[] actors = GameObject.FindObjectsByType<GenericBActor>(FindObjectsInactive.Exclude, FindObjectsSortMode.None); // find actors

        for (int i = 0; i < actors.Length; i++) {
            if (actors[i].GetComponent<PlayerOV>() != null) actors[i].GetComponent<PlayerOV>().enabled = false;
            actors[i].enabled = false;
        }

        StartCoroutine(SaveMenuOnSequence());
    }
}
