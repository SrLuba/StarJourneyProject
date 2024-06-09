using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_UISelector_Input : MonoBehaviour
{
    public Battle_UISelector selector;
    public AudioClip moveClip, AcceptClip, UnallowedClip;


    void Update()
    {
        if (BattleManager.instance == null) return;
        if (BattleManager.instance.currentTurn.charaType != CharaType.Player) return;
        if (InputManager.instance.rPress) {
            selector.ActionID++;
            SoundManager.instance.Play(moveClip);
            if (selector.ActionID > selector.options.Count - 1) selector.ActionID = 0;
        }
        else if (InputManager.instance.lPress)
        {
            selector.ActionID--;
            SoundManager.instance.Play(moveClip);
            if (selector.ActionID < 0) selector.ActionID = selector.options.Count - 1;
        }
        
      

}
}
