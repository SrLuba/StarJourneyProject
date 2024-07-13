using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public enum MenuActionType { 
    LoadScene,
    LoadLevel,
    ChangeMenuBYID,
    AddPage,
    SubPage,
    Quit,
    VariableChange,
    PlayerTypeToggle
}
[System.Serializable]
public class MenuAction {
    public MenuActionType type;
    public string action;
    public byte IDReference;

    public void DoAction(MenuController controller) {
        if (type == MenuActionType.LoadScene)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(action);
        }
        else if (type == MenuActionType.ChangeMenuBYID)
        {
            controller.ChangePageByID(this.IDReference);

        }
        else if (type == MenuActionType.Quit)
        {
            Application.Quit();
        }
        else if (type == MenuActionType.LoadLevel)
        {
           // StaticManager.instance.selectedMap = Resources.Load<MapSO>("Maps/" + action);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Overworld");
        }
    }
}
public class MenuItemController : MonoBehaviour
{
    public bool canClick = true;
    public MenuAction action;
    public AudioClip actionSound;
    public TMP_Text text;
    string targetText = "";

    public void Start()
    {
        if (text == null) text = this.GetComponent<TMP_Text>();
        targetText = text.text;
    }

    public void Update()
    {
      
    }

    public void Click(MenuController controller) {
        if (actionSound!=null) SoundManager.instance.Play(actionSound);
        action.DoAction(controller);

    }
}
