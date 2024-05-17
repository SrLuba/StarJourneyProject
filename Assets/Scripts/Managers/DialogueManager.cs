using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    public List<DialogueSO> dialogues;
    public GameObject DialogBox;
    public RectTransform DialogBoxT;
    public RectTransform CanvasRect;
    public Image ArrowImage;
    public Image boxBehind;
    public TMP_Text txtPH;

    public string currentInstructionName = "UNKNOWN";
    public string nextInstructionName = "UNKNOWN";
    public string lastInstructionName = "UNKNOWN";
    public string currentState = "IDLE";
    public string currentDialog = "UNKNOWN";


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
        DialogBox.SetActive(false);
        //DisplayDialog("Test_Dialog");
    }

    public void DisplayDialog(string identifier) {
        currentState = "DISPLAY DIALOG - " + identifier.ToUpper();
        StartCoroutine(DisplayDialogue(identifier));
    }
    public IEnumerator DisplayDialogue(string identifier)
    {
        DialogueSO a = dialogues.Find(x => x.identifier.ToUpper() == identifier.ToUpper());


        if (a != null)
        {

            


            currentDialog = a.name;
            currentState = "FOUND DIALOGUE";
            for (int i = 0; i < a.dialogue.text.Count; i++)
            {
                currentState = "SET DIALOGUE BOX ACTIVE (TRUE)";
                DialogBox.SetActive(true);

          
                yield return new WaitForSeconds(0.02f);

                SoundManager.instance.Play(a.dialogue.text[i].character.SpeechBubbleSFX, true, "DIALOG");
                currentState = "Start SFX and DisplayText - I" + i.ToString();
                // Restrict Player 
                EventManager.instance.onEvent = true;
                OVManager.instance.mainPlayer.canInput = false;
                OVManager.instance.mainPlayer.input = new Vector2(0f, 0f);
                OVManager.instance.mainPlayer.animV = a.dialogue.mainPlayerOr;

                if (OVManager.instance.secondaryPlayer != null)
                {
                    OVManager.instance.secondaryPlayer.canInput = false;
                    OVManager.instance.secondaryPlayer.input = new Vector2(0f, 0f);
                    OVManager.instance.secondaryPlayer.animV = a.dialogue.secondaryPlayerOr;
                }

                OVManager.instance.mainCamera.ResetControl();

                yield return a.dialogue.DisplayText(i, boxBehind, CanvasRect, ArrowImage, DialogBoxT, txtPH);

                SoundManager.instance.Stop(a.dialogue.text[i].character.SpeechBubbleSFX, "DIALOG");
                currentState = "Done Displaying Dialogue - Waiting User Input";
                while (!InputManager.instance.aHold)
                {
                    yield return new WaitForSeconds(0.01f);
                }
                currentState = "Done Displaying Dialogue - Got User Input";
                DialogBox.SetActive(false);

                if (a.dialogue.text[i].afterEvent != null) yield return EventManager.instance.StartEvent(a.dialogue.text[i].afterEvent.name);
                
                yield return new WaitForSeconds(0.1f);

            }
            EventManager.instance.onEvent = false;
            OVManager.instance.mainPlayer.canInput = true;
            OVManager.instance.mainPlayer.AI = false;

            if (OVManager.instance.secondaryPlayer != null)
            {
                OVManager.instance.secondaryPlayer.canInput = true;
                OVManager.instance.secondaryPlayer.AI = true;
            }
            OVManager.instance.mainCamera.ResetControl();

            currentState = "SET DIALOGUE BOX ACTIVE (FALSE)";
            DialogBox.SetActive(false);
      
        }
        else {
            currentDialog = "UNKNOWN";
        }
        currentState = "IDLE";

     

    }
}
