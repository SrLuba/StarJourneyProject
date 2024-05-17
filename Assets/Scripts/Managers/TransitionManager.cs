using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class TransitionInfo {
    public string identifier = "DEFAULT";
    public GameObject tIn, tOut;
    public float inDuration, outDuration;
    public AudioClip inClip, outClip;

    public IEnumerator doTransition(Camera cam) {
        GameObject inTemp = MonoBehaviour.Instantiate(tIn, cam.transform.position, Quaternion.identity);
        inTemp.transform.eulerAngles = cam.transform.eulerAngles;
        inTemp.transform.Translate(0f, 0f, 1f);


        inTemp.transform.SetParent(cam.transform);

        if (inClip != null) { SoundManager.instance.Play(inClip); }

        yield return new WaitForSeconds(inDuration+0.25f);
        MonoBehaviour.Destroy(inTemp);

        GameObject outTemp = MonoBehaviour.Instantiate(tOut, cam.transform.position, Quaternion.identity);
        outTemp.transform.eulerAngles = cam.transform.eulerAngles;
        outTemp.transform.Translate(0f, 0f, 1f);

        outTemp.transform.SetParent(cam.transform);

        if (outClip != null) { SoundManager.instance.Play(outClip); }

        yield return new WaitForSeconds(outDuration);
        MonoBehaviour.Destroy(outTemp);
    }
}

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager instance;

    public Camera camera;

    public List<TransitionInfo> transitions;

    public Animator BattleA;

    public AudioClip NormalBTClip, DisadvantageBTClip;




    public void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
    }
    public IEnumerator StartTransitionBattle(BattleEnteringCase enteringCase, bool player1contact, BattleSO so) {
        if (OVManager.instance.mainPlayer != null) OVManager.instance.mainPlayer.PrepareForBattle(enteringCase == BattleEnteringCase.Advantage && player1contact);
        if (OVManager.instance.secondaryPlayer != null) OVManager.instance.secondaryPlayer.PrepareForBattle(enteringCase == BattleEnteringCase.Advantage && !player1contact);

        MusicManager.instance.StopAll();
        
        StaticManager.instance.goBattle = so;



        AudioClip a = (enteringCase == BattleEnteringCase.Disadvantage) ? DisadvantageBTClip : NormalBTClip;
        SoundManager.instance.Play(a);
        yield return new WaitForSeconds(0.2f);
        MusicManager.instance.PlayClip(so.music.musicStart, false);
        OVManager.instance.mainCamera.CameraTransitionateBTL();
        if (OVManager.instance.playerType == 2)
        {
            if (!OVManager.instance.secondaryPlayer.split) { 
                if (player1contact)
                {
                    SoundManager.instance.Play(OVManager.instance.player.battleEntrance_VoiceClips.getClip(enteringCase));
                    yield return new WaitForSeconds(OVManager.instance.player.battleEntrance_VoiceClips.getClip(enteringCase).length - 0.5f);
                    SoundManager.instance.Play(OVManager.instance.player2.battleEntrance_VoiceClips.getClip(enteringCase));
                    yield return new WaitForSeconds(OVManager.instance.player2.battleEntrance_VoiceClips.getClip(enteringCase).length - 0.5f);
                }
                else
                {
                    SoundManager.instance.Play(OVManager.instance.player2.battleEntrance_VoiceClips.getClip(enteringCase));
                    yield return new WaitForSeconds(OVManager.instance.player2.battleEntrance_VoiceClips.getClip(enteringCase).length - 0.5f);
                    SoundManager.instance.Play(OVManager.instance.player.battleEntrance_VoiceClips.getClip(enteringCase));
                    yield return new WaitForSeconds(OVManager.instance.player.battleEntrance_VoiceClips.getClip(enteringCase).length - 0.5f);
                }
            }
            else
            {
                SoundManager.instance.Play(OVManager.instance.player.battleEntrance_VoiceClips.getClip(enteringCase));
                yield return new WaitForSeconds(OVManager.instance.player.battleEntrance_VoiceClips.getClip(enteringCase).length - 0.5f);
            }
        }
        else
        {
            SoundManager.instance.Play(OVManager.instance.player.battleEntrance_VoiceClips.getClip(enteringCase));
            yield return new WaitForSeconds(OVManager.instance.player.battleEntrance_VoiceClips.getClip(enteringCase).length - 0.5f);
        }
      
        yield return new WaitForSeconds(0.5f);
        if (enteringCase != BattleEnteringCase.Disadvantage) { 
            BattleA.Play("Transition_On_" + enteringCase.ToString() + "_" + (StaticManager.instance.company ? "Company" : "Solo") + "_" + StaticManager.instance.battleAdvantageCase.ToString() + "_" + (((player1contact && StaticManager.instance.marioAhead)) ? "M" : "L")); }
        else {
            if (StaticManager.instance.company) { BattleA.Play("Transition_On_Disadvantage_" + (StaticManager.instance.marioAhead ? "L" : "M")); }
            else { BattleA.Play("Transition_On_Disadvantage_Solo_" + (StaticManager.instance.marioAhead ? "M" : "L")); }
        }

        yield return new WaitForSeconds(so.music.musicStart.length-so.music.musicOffset);
        MusicManager.instance.PlayClip(so.music.musicLoop, true);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Battle");
    }
    public void StartTransition(string identifier) {
        TransitionInfo inf = transitions.Find(x => x.identifier.ToUpper() == identifier.ToUpper());

        if (inf == null) return;

        StartCoroutine(inf.doTransition(this.camera));
    }

    void Update()
    {
        if (camera == null)
        {
            camera = Camera.main;
            return;
        }
    }
}
