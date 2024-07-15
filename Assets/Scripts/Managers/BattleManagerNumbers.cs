using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class BattleManagerNumbers : MonoBehaviour
{
    public static BattleManagerNumbers instance;

    public GameObject hurtKindPrefab;
    public GameObject rankingKindPrefab;

    public Camera mainCam;
    public RectTransform mainCanvas;

    public Vector2 hurtUIOffset;

    public float constantUserValue = 0.5f;

    RankingSO ranking;


    public void Awake()
    {
        instance = this;
    }
    public void Start()
    {
        ranking = Resources.Load<RankingSO>("Data/MainRanking");
    }
    public void Hurt(int value, BattleActorSO character)
    {
        GameObject characterGB = character.getInstance();
        if (characterGB == null) return;
        if (character.dead) return;

        characterGB.GetComponent<GenericBActor>().StartCoroutine(characterGB.GetComponent<GenericBActor>().Hurt(25f));
        GameObject hurtGB = Instantiate(hurtKindPrefab, characterGB.transform.position, Quaternion.identity);
        UIHurtMeter uiHM = hurtGB.GetComponent<UIHurtMeter>();

        uiHM.Init(mainCanvas, value);
        hurtGB.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        hurtGB.transform.position = mainCanvas.transform.position;

        Vector2 ViewportPosition = mainCam.WorldToViewportPoint(characterGB.transform.position);

        Vector2 WorldObject_ScreenPosition = hurtUIOffset + new Vector2(
        ((ViewportPosition.x * mainCanvas.sizeDelta.x) - (mainCanvas.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * mainCanvas.sizeDelta.y) - (mainCanvas.sizeDelta.y * 0.5f)));
        hurtGB.GetComponent<RectTransform>().anchoredPosition = WorldObject_ScreenPosition;

        character.stats.HEALTH.currentValue -= value;
        if (character.stats.HEALTH.currentValue < 0) character.stats.HEALTH.currentValue = 0;
    }
    public void Hurt(int value, BattleActorSO source, BattleActorSO character) {
        GameObject characterGB = character.getInstance();
        if (characterGB == null) return;
        if (character.dead) return;

        characterGB.GetComponent<GenericBActor>().StartCoroutine(characterGB.GetComponent<GenericBActor>().Hurt(source.stunForce));
        GameObject hurtGB = Instantiate(hurtKindPrefab, characterGB.transform.position, Quaternion.identity);
        UIHurtMeter uiHM = hurtGB.GetComponent<UIHurtMeter>();

        uiHM.Init(mainCanvas, value);
        hurtGB.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        hurtGB.transform.position = mainCanvas.transform.position;

        Vector2 ViewportPosition = mainCam.WorldToViewportPoint(characterGB.transform.position);

        Vector2 WorldObject_ScreenPosition = hurtUIOffset + new Vector2(
        ((ViewportPosition.x * mainCanvas.sizeDelta.x) - (mainCanvas.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * mainCanvas.sizeDelta.y) - (mainCanvas.sizeDelta.y * 0.5f)));
        hurtGB.GetComponent<RectTransform>().anchoredPosition = WorldObject_ScreenPosition;

        character.stats.HEALTH.currentValue -= value;
        if (character.stats.HEALTH.currentValue < 0) character.stats.HEALTH.currentValue = 0;
    }
    public void ShowRanking(int R)
    {
        int rankingN = Mathf.Clamp(R, 0, ranking.rankings.Count - 1);
        GameObject g = Instantiate(ranking.rankings[rankingN].Prefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        g.transform.SetParent(mainCanvas);
        g.transform.localPosition = Vector3.zero + ranking.rankings[rankingN].offset;
        g.transform.eulerAngles = Vector3.zero;
        g.transform.localEulerAngles = Vector3.zero;
        g.transform.localScale = new Vector3(1f, 1f, 1f);
        g.transform.GetChild(0).GetComponent<Image>().sprite = ranking.rankings[rankingN].assignedSprite;
        g.transform.GetChild(0).GetComponent<Image>().SetNativeSize();

        SoundManager.instance.Play(ranking.rankings[rankingN].sound);
        if (rankingN >= 3) StartCoroutine(ExcellentRanking(0.6f));

    }

    public IEnumerator ExcellentRanking(float time) {
        CinematicManager.instance.excellent = true;
        yield return new WaitForSeconds(time);
        CinematicManager.instance.excellent = false;
    }
    public void Update()
    {
        if (Keyboard.current.hKey.wasPressedThisFrame) {
            BattleActorSO result = BattleManager.instance.playerActors[Random.Range(0, BattleManager.instance.playerActors.Count-1)];
            this.Hurt(Random.Range(1,25), result);
        }
    
    }
}
