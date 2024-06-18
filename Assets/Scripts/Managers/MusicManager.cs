using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public MusicSO myMusic;
    public AudioSource mainSource;

    public bool debug = true;
    public bool debugBPM = true;
    public AudioClip BPMTick;

    public bool enableBPM = true;

    public float timerBPM;


    public void ResetBPM() {
        timerBPM = 0f;
    }
    /*
    Void BEAT()
    this is called every beat (calculated via BPM)
 */
    public void Beat() {
       
        if (!enableBPM) return;
        if (debugBPM) { SoundManager.instance.Play(this.BPMTick); } // We play the usual tempo tick sound to test bpm the right way
        ResetBPM();


        if (Battle_Camera.instance!=null) Battle_Camera.instance.Beat(); // we call the beat function from the battle camera
    }
    public void UpdateBeat() {
        if (!enableBPM) {
            ResetBPM();
            return;
        }

        /*
            BPM Usage / There's 60 seconds in a minute
            BPM = Beats Per Minute that can be easily calculated dividing bpm by 60
         */
        float bps = (myMusic.bpm / 60f);

        if (timerBPM >= 1f / bps) {
            Beat();
        }

        timerBPM += Time.deltaTime;
    }

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
        DontDestroyOnLoad(this.mainSource.gameObject);

        if (myMusic != null) {
            this.UpdateBeat();
            myMusic.CheckForLoop(mainSource, 0); 
        }
    }
    public void Start()
    {
        this.transform.SetParent(null);
        this.transform.localPosition = new Vector3(0f, 0f, 0f);
       
        if (debug) this.PlayClip(this.myMusic, true);
    }
   
    public void PlayClip(MusicSO mso, bool loop) {
        mainSource.Stop();
        mainSource.clip = mso.music;
        mainSource.loop = loop;
        mainSource.volume = 1f;
        mainSource.Play();
        myMusic = mso;
        timerBPM = 0f;
        ResetBPM();
    }
    public void StopAll()
    {
        mainSource.Stop();
        timerBPM = 0f;
        ResetBPM();
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

