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
        if (BattleManager.instance.currentTurn != null) { if (BattleManager.instance.currentTurn.linkedActor.myType != ActorType.Player) { return; } }
        if (!selector.active) return;

        if (InputManager.instance.engine.getPressed("RIGHT")) {
            selector.ActionID++;
            SoundManager.instance.Play(moveClip);
            if (selector.ActionID > selector.options.Count - 1) selector.ActionID = 0;
        }
        else if (InputManager.instance.engine.getPressed("LEFT"))
        {
            selector.ActionID--;
            SoundManager.instance.Play(moveClip);
            if (selector.ActionID < 0) selector.ActionID = selector.options.Count - 1;
        }
        
      

}
}
