
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sound Package, it works for having Sound Packaged, very straight forward
// You create a file with a theme in common and you place every sound file into a package, then on code calling by ID and Main Package.

[System.Serializable]
public class SoundPackage {
    public string identifier = "thing";

    public List<AudioClip> clipList;

    public AudioClip getClip() {
        if (clipList.Count==1) return clipList[0];

        return clipList[Random.Range(0, clipList.Count)];
    }
}
[CreateAssetMenu]
public class SoundPKG : ScriptableObject
{
    public List<SoundPackage> packages;
    public AudioClip defaultClip;

    public AudioClip getClip(string identifier)
    {
        SoundPackage SFXPKG = packages.Find(x => x.identifier.ToUpper() == identifier.ToUpper());

        if (SFXPKG == null)
        {
            Debug.Log("Couldn't find Clip(" + identifier.ToUpper() + ") on " + this.name.ToUpper());
            return defaultClip;
        }
        return SFXPKG.getClip();
    }
}
