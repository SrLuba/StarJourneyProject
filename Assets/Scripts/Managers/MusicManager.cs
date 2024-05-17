using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public AudioSource mainSource;


    public void Awake()
    {
        if (instance != null) {
            Destroy(instance.gameObject);
        }
        instance = this;
    }
    public void Update()
    {
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(mainSource.gameObject);
    }
    public void Start()
    {
        
       


        this.transform.SetParent(null);
        this.transform.localPosition = new Vector3(0f, 0f, 0f);
    }
   
    public void PlayClip(AudioClip clip, bool loop) {
        mainSource.Stop();
        mainSource.clip = clip;
        mainSource.loop = loop;
        mainSource.volume = 1f;
        mainSource.Play();
    }
    public void StopAll()
    {
        mainSource.Stop();
    }
    public void BattleStart(BattleEnteringCase eCase, MusicSO mso)
    {
        StartCoroutine(BattleMusicStart(eCase, mso));
    }

    public IEnumerator BattleMusicStart(BattleEnteringCase eCase, MusicSO mso) {
        if (eCase != BattleEnteringCase.NoTransition)
        {
            this.PlayClip(mso.musicStart, false);
            yield return new WaitForSeconds(mso.musicStart.length+mso.musicOffset);
            BattleManager.instance.transitionAnim.Play("Transition_End");
            this.PlayClip(mso.musicLoop, false);
        }
        else {
            this.PlayClip(mso.musicLoop, false);
        }
    }
}
