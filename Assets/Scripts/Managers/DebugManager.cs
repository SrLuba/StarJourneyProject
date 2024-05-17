using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class DebugManager : MonoBehaviour
{
    public static DebugManager instance;
    public bool debugDialogue, debugOV;
    public GameObject debugDialogueCanvas, debugOVCanvas;


    public TMP_Text debugDialogue_UI_CurrentState, debugDialogue_UI_CurrentDialog, debugDialogue_UI_CurrentInstruction, debugDialogue_UI_NextInstruction, debugDialogue_UI_LastInstruction;
    public TMP_InputField debugDialogue_UI_OverrideField;

    public TMP_Text debugOV_PriPos, debugOV_SecPos, debugOV_GeneralInfo;
    public Toggle debugOV_PriFloor, debugOV_SecFloor;

    public TMP_Text debugOV_P1Anim, debugOV_P2Anim, debugOV_Split, debugOV_OnEvent;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void StartDialogue() {
        string t = debugDialogue_UI_OverrideField.text.ToUpper();

        DialogueManager.instance.DisplayDialog(t);
    }
    public void UpdateDebug_Dialogue() {
        if (!debugDialogue) return;

        debugDialogue_UI_CurrentState.text = DialogueManager.instance.currentState;
        debugDialogue_UI_CurrentInstruction.text = DialogueManager.instance.currentInstructionName;
        debugDialogue_UI_NextInstruction.text = DialogueManager.instance.nextInstructionName;
        debugDialogue_UI_LastInstruction.text = DialogueManager.instance.lastInstructionName;
        debugDialogue_UI_CurrentDialog.text = DialogueManager.instance.currentDialog;
    }
    public void UpdateDebug_OV() {
        if (!debugOV) return;
        debugOV_GeneralInfo.text = "PLAYER COUNT / TYPE = " + OVManager.instance.playerType.ToString().ToUpper();
        if (OVManager.instance.secondaryPlayer != null)
        {
            debugOV_SecPos.text = "Secondary Player Position (" + OVManager.instance.secondaryPlayer.transform.position.x.ToString() + "," + OVManager.instance.secondaryPlayer.transform.position.y.ToString() + "," + OVManager.instance.secondaryPlayer.transform.position.z.ToString() + ",)";
            debugOV_SecFloor.isOn = OVManager.instance.secondaryPlayer.Grounded;
            debugOV_P2Anim.text = OVManager.instance.secondaryPlayer.anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            debugOV_Split.text = (OVManager.instance.secondaryPlayer.split) ? "SPLIT STATE - SPLIT" : "SPLIT STATE - COMPANY";
        }
        debugOV_PriPos.text = "Main Player Position ("+ OVManager.instance.mainPlayer.transform.position.x.ToString() + ","+ OVManager.instance.mainPlayer.transform.position.y.ToString() + ","+ OVManager.instance.mainPlayer.transform.position.z.ToString() + ",)";
        debugOV_PriFloor.isOn = OVManager.instance.mainPlayer.Grounded;
        debugOV_P1Anim.text = OVManager.instance.mainPlayer.anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        debugOV_OnEvent.text = (EventManager.instance.onEvent) ? "EVENTMANAGERSTATE - ONEVENT" : "EVENTMANAGERSTATE - IDLE";
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.dKey.isPressed && Keyboard.current.digit1Key.wasPressedThisFrame) debugDialogue = !debugDialogue;
        if (Keyboard.current.dKey.isPressed && Keyboard.current.digit2Key.wasPressedThisFrame) debugOV = !debugOV;

        if (debugDialogue) UpdateDebug_Dialogue();
        if (debugOV) UpdateDebug_OV();

        debugDialogueCanvas.SetActive(debugDialogue);
        debugOVCanvas.SetActive(debugOV);
    }
}
