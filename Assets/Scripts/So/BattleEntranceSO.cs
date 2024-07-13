using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceLineReference {
    public string identifier;
    public List<AudioClip> clips;

    public void Play(List<string> characterLookUp) {
        if (!characterLookUp.Contains(identifier)) return;
        SoundManager.instance.Play(clips[Random.Range(0, clips.Count - 1)]);
    }
}
[CreateAssetMenu]
public class BattleEntranceSO : MonoBehaviour
{
    public string description = "????";
    public List<VoiceLineReference> voiceLinesReferences;
}
