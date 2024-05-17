using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static SoundManager instance;

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
    }
    public void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public void Play(AudioClip clip) {
        GameObject g = new GameObject("Sound_" + clip.name);
        AudioSource src = g.AddComponent<AudioSource>();
        src.clip = clip;
        src.loop = false;
        src.Play();
        g.AddComponent<SoundController>();
        g.transform.SetParent(this.transform);
    }
    public void Stop(AudioClip clip, string pTag) {
        GameObject G = GameObject.Find("Sound_" + clip.name + "_" + pTag);
        if (G == null) return;
        Destroy(G);
    }
    public void Play(AudioClip clip, bool loop, string pTag)
    {
        GameObject g = new GameObject("Sound_" + clip.name + "_" + pTag);
        AudioSource src = g.AddComponent<AudioSource>();
        src.clip = clip;
        src.loop = loop;
        src.Play();
        g.transform.SetParent(this.transform);
    }
    public void PlayFoley(List<AudioClip> clips, float minPitch, float maxPitch)
    {
        AudioClip clip = clips[Random.Range(0, clips.Count - 1)];
        GameObject g = new GameObject("Foley_" + clip.name);
        AudioSource src = g.AddComponent<AudioSource>();
        src.clip = clip;
        src.pitch = Random.Range(minPitch, maxPitch);
        src.loop = false;
        src.Play();
        g.AddComponent<SoundController>();
        g.transform.SetParent(this.transform);
    }
}
