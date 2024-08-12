using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static SoundManager instance;

    public List<SoundPKG> soundPackages;
    public AudioClip defaultAudioClip;

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


        soundPackages = new List<SoundPKG>(Resources.LoadAll<SoundPKG>("Data/SoundData/"));

    }

    public AudioClip getClipFromLibrary(string expression) {

        string groupID = expression.Split('|')[0].ToUpper();
        string foleyID = expression.Split('|')[1].ToUpper();

        SoundPKG mySoundPackage = this.soundPackages.Find(x => x.name.ToUpper() == groupID.ToUpper());

        if (mySoundPackage == null)
        {
            Debug.Log("Couldn't get Clip! (" + expression + ")");
            return this.defaultAudioClip;
        }
        return mySoundPackage.getClip(foleyID.ToUpper());
    }
    public void Play(string ID, bool loop) {
        AudioClip myClip = this.getClipFromLibrary(ID.ToUpper());


        GameObject g = new GameObject("Sound_" + myClip.name);
        AudioSource src = g.AddComponent<AudioSource>();
        src.clip = myClip;
        src.loop = loop;
        src.Play();
        g.AddComponent<SoundController>();
        g.transform.SetParent(this.transform);
    }
    public void PlayVoiceLine(string characterID, string soundID)
    {

        string expressionID = ("character_" + characterID + "_voicelines|" + soundID).ToUpper();
        AudioClip myClip = this.getClipFromLibrary(expressionID);


        GameObject g = new GameObject("Sound_" + myClip.name);
        AudioSource src = g.AddComponent<AudioSource>();
        src.clip = myClip;
        src.loop = false;
        src.Play();
        g.AddComponent<SoundController>();
        g.transform.SetParent(this.transform);
    }
}
