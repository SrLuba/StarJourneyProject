using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public MusicSO myMusic;
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

        if (myMusic!=null) myMusic.CheckForLoop(mainSource, 0);
    }
    public void Start()
    {
        this.transform.SetParent(null);
        this.transform.localPosition = new Vector3(0f, 0f, 0f);
    }
   
    public void PlayClip(MusicSO mso, bool loop) {
        mainSource.Stop();
        mainSource.clip = mso.music;
        mainSource.loop = loop;
        mainSource.volume = 1f;
        mainSource.Play();
        myMusic = mso;
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
            this.PlayClip(mso, false);

            yield return new WaitForSeconds(0.01f);

            BattleManager.instance.transitionAnim.Play("Transition_End");   
        }
    }
}
