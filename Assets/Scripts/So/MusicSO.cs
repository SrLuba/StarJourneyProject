using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class MusicSO : ScriptableObject
{
    [Header("Music Loop")] public AudioClip music;

    public List<Vector2> musicOffset;

    public float bpm = 120f;

    public void Play(AudioSource source) {
        source.clip = music;
        source.Play();
    }
    public void CheckForLoop(AudioSource source, int id)
    {
        if (source.clip != music) return;

        if (source.time >= musicOffset[id].y) {
            PlayLoop(source, id);
        }
    }
    public void PlayLoop(AudioSource source, int id) {
        source.time = musicOffset[id].x;
    }
}
