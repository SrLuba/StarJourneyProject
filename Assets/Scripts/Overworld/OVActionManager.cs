using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OVActionManager : MonoBehaviour
{
    public static OVActionManager instance;

    public int actionID_Player, actionID_Adjacent;

    public int playerID = 0;

    public RectTransform MT, LT;

    public float sizeA, sizeB;

    public AudioClip changeClip;

    public bool busyPerk = false;




    public void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (busyPerk) return;

        MT.sizeDelta = new Vector2((Keyboard.current.aKey.isPressed) ? sizeA : sizeB, (Keyboard.current.aKey.isPressed) ? sizeA : sizeB);
        LT.sizeDelta = new Vector2((Keyboard.current.sKey.isPressed) ? sizeA : sizeB, (Keyboard.current.sKey.isPressed) ? sizeA : sizeB);
        
        if (InputManager.instance.lPress)
        {
            actionID_Player++;
            if (actionID_Player > 1) actionID_Player = 0;
            SoundManager.instance.Play(changeClip);
            if (OVManager.instance.player.OVActor.perks[actionID_Player].selfClipWhenSelect!=null) SoundManager.instance.Play(OVManager.instance.player.OVActor.perks[actionID_Player].selfClipWhenSelect);
            if (OVManager.instance.player.OVActor.perks[actionID_Player].adjacentClipWhenSelect!=null) SoundManager.instance.Play(OVManager.instance.player.OVActor.perks[actionID_Player].adjacentClipWhenSelect);
        }
     
        if (OVManager.instance.playerType == 2 && !OVManager.instance.secondaryPlayer.split)
        {
            if (InputManager.instance.rPress)
            {
                actionID_Adjacent++;
                if (actionID_Adjacent > 1) actionID_Adjacent = 0;
                SoundManager.instance.Play(changeClip);
                if (OVManager.instance.player2.OVActor.perks[actionID_Adjacent].selfClipWhenSelect!=null) SoundManager.instance.Play(OVManager.instance.player2.OVActor.perks[actionID_Adjacent].selfClipWhenSelect);
                if (OVManager.instance.player2.OVActor.perks[actionID_Adjacent].adjacentClipWhenSelect!=null) SoundManager.instance.Play(OVManager.instance.player2.OVActor.perks[actionID_Adjacent].adjacentClipWhenSelect);
            }
        }
        else {
            actionID_Adjacent = 0;
        }
    }
}
