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

    public Sprite HammerTransitionBG;
    public Sprite JumpTransitionBG;
    public Sprite NormalTransitionBG;
    public Sprite DisadvantageTransitionBG;

    public SpriteRenderer transitionBG;

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

        Sprite actualBG = NormalTransitionBG;

        if (enteringCase == BattleEnteringCase.Disadvantage) actualBG = DisadvantageTransitionBG;
        if (enteringCase == BattleEnteringCase.Advantage) actualBG = (StaticManager.instance.battleAdvantageCase == 2) ? HammerTransitionBG : JumpTransitionBG;

        transitionBG.sprite = actualBG;

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
                    int randR = Random.Range(0, 2);
                    SoundManager.instance.Play(OVManager.instance.player.battleEntrance_VoiceClips.getClip(enteringCase)[randR]);
                    yield return new WaitForSeconds(OVManager.instance.player.battleEntrance_VoiceClips.getClip(enteringCase)[randR].length - 0.5f);
                    int randR2 = Random.Range(0, 2);
                    SoundManager.instance.Play(OVManager.instance.player2.battleEntrance_VoiceClips.getClip(enteringCase)[randR2]);
                    yield return new WaitForSeconds(OVManager.instance.player2.battleEntrance_VoiceClips.getClip(enteringCase)[randR2].length - 0.5f);
                }
                else
                {
                    int randR3 = Random.Range(0, 2);
                    SoundManager.instance.Play(OVManager.instance.player.battleEntrance_VoiceClips.getClip(enteringCase)[randR3]);
                    yield return new WaitForSeconds(OVManager.instance.player2.battleEntrance_VoiceClips.getClip(enteringCase)[randR3].length - 0.5f);
                    int randR4 = Random.Range(0, 2);
                    SoundManager.instance.Play(OVManager.instance.player2.battleEntrance_VoiceClips.getClip(enteringCase)[randR4]);
                    yield return new WaitForSeconds(OVManager.instance.player.battleEntrance_VoiceClips.getClip(enteringCase)[randR4].length - 0.5f);
                }
            }
            else
            {
                int randR5 = Random.Range(0, 2);
                SoundManager.instance.Play(OVManager.instance.player.battleEntrance_VoiceClips.getClip(enteringCase)[randR5]);
                yield return new WaitForSeconds(OVManager.instance.player.battleEntrance_VoiceClips.getClip(enteringCase)[randR5].length - 0.5f);
            }
        }
        else
        {
            int randR6 = Random.Range(0, 2);
            SoundManager.instance.Play(OVManager.instance.player.battleEntrance_VoiceClips.getClip(enteringCase)[randR6]);
            yield return new WaitForSeconds(OVManager.instance.player.battleEntrance_VoiceClips.getClip(enteringCase)[randR6].length - 0.5f);
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
