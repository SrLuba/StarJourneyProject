using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

[System.Serializable]
public class DialogueTextItem
{
    public CharaSO character;
    public string text;
    public Sprite textBoxSprite; 

    public Sprite arrowSprite;
    public Vector2 arrowOffset;

    public Vector2 boxOffset;
    public Vector2 boxSize;
    public Vector2 textOffset, textSize;

    public float delayBetweenChars;
    public EventSO afterEvent;


}
[System.Serializable] public class Dialogue {

    public string identifier = "E";
    public List<DialogueTextItem> text;

    public float arrowDistance;

    public Vector2 mainPlayerOr, secondaryPlayerOr;
    
    public IEnumerator DisplayText(int id, Image BGBox, RectTransform CanvasRect, Image tArrow, RectTransform tBox, TMP_Text txt)
    {
        char[] cText = text[id].text.ToCharArray();
        txt.text = "";


        bool buttonPressed = false;
        Transform actor = text[id].character.findSelf();
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint((actor != null) ? actor.position : text[id].boxOffset);
        Vector2 WorldObject_ScreenPosition = new Vector2(
            ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
            ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

        tBox.anchoredPosition = WorldObject_ScreenPosition + ((actor == null) ? new Vector2(0f, 0f) : (Vector2)text[id].boxOffset);
        tBox.sizeDelta = text[id].boxSize;

        Rect rect = new Rect(text[id].textOffset, text[id].textSize);

        txt.rectTransform.sizeDelta = rect.size;
        txt.transform.localPosition = rect.position;

        BGBox.sprite = text[id].textBoxSprite;
        BGBox.SetNativeSize();
        BGBox.rectTransform.sizeDelta = text[id].boxSize;



        tArrow.sprite = text[id].arrowSprite;
        tArrow.SetNativeSize();
        tArrow.transform.localPosition = text[id].arrowOffset;
        tArrow.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);



        for (int i = 0; i < cText.Length; i++) { 
            if (InputManager.instance.bHold)
            {
                txt.text = text[id].text;
                buttonPressed = true;
                break;
            }

            txt.text += cText[i];
            
            yield return new WaitForSeconds(text[id].delayBetweenChars);
        }

        if (buttonPressed) {
            SoundManager.instance.Play(text[id].character.SpeechBubbleSkipSFX);
        }


    }
}
[CreateAssetMenu]
public class DialogueSO : ScriptableObject
{
    public string identifier;
    public Dialogue dialogue;
}
