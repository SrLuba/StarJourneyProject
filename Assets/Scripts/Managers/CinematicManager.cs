using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CinematicManager : MonoBehaviour
{
    public static CinematicManager instance;

    public bool blackLines = false;
    public RectTransform blackLine_Top, BlackLine_Bottom;

    public Image bgExcellent;
    public bool excellent;
    public void Awake()
    {
        instance = this;
    }
    public void Update()
    {
        BlackLine_Bottom.anchoredPosition = new Vector3(0f, Mathf.Lerp(BlackLine_Bottom.anchoredPosition.y, (!blackLines) ? -129f : 0f, 5f * Time.deltaTime), 0f);
        blackLine_Top.anchoredPosition = new Vector3(0f, Mathf.Lerp(blackLine_Top.anchoredPosition.y, (!blackLines) ? 129f : 0f, 5f * Time.deltaTime), 0f);
        bgExcellent.color = Color.Lerp(bgExcellent.color, (excellent) ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f), 7f * Time.deltaTime);
    }
}
