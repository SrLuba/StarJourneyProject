using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StepType {
    public string identifier;
    public List<AudioClip> clips;
}

public class FootController : MonoBehaviour
{

    public string floorType = "GRASS";

    public ParticleSystem smoke;

    public List<StepType> stepTypes;
    public StepType defaultStepType;

    public bool wet;
    public GameObject wetFootObject;
    public Vector3 wetFootoffset;

    public void StepFoot() {
        StepType s = stepTypes.Find(x => x.identifier.ToUpper() == floorType.ToUpper());

        if (s == null) { SoundManager.instance.PlayFoley(defaultStepType.clips, 0.9f, 1.1f); }
        else { SoundManager.instance.PlayFoley(s.clips, 0.9f, 1.1f); }

        if (smoke!=null)smoke.Play();
        if (wet && wetFootObject != null) {
            Instantiate(wetFootObject, this.transform.position + wetFootoffset, Quaternion.identity);
        }
    }
}
