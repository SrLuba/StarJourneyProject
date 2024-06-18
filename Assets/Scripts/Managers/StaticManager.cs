using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PlayerTestType { 
    Mario,
    Luigi,
    MarioAndLuigi,
    LuigiAndMario,
    GoombaFRFR
}

public class StaticManager : MonoBehaviour
{
    public static StaticManager instance;

    public bool onBattle;
    public int battleEnteringCase;

    public bool marioAhead = false;

    public int battleAdvantageCase = 0;
    public bool company = false;

    public BattleSO goBattle;

    public List<CharaSO> players = new List<CharaSO>();

    public MapSO selectedMap;



    public PlayerTestType playerTestType = PlayerTestType.MarioAndLuigi;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
       
    }
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
      
    }
    private void Update()
    {
        if (!onBattle) {
            if (OVManager.instance == null) return;
            if (OVManager.instance.playerType == 2)
            {
                company = !OVManager.instance.secondaryPlayer.split;
            }
            else {
                company = false;
            }
        }
        if (OVManager.instance != null)
        {
            CharaSO player1 = Resources.Load<CharaSO>("Data/Players/Player_1");
            CharaSO player2 = Resources.Load<CharaSO>("Data/Players/Player_2");


            switch (playerTestType)
            {
                case PlayerTestType.Mario:
                    OVManager.instance.player = player1;
                    OVManager.instance.player2 = null;
                    break;
                case PlayerTestType.Luigi:
                    OVManager.instance.player = player2;
                    OVManager.instance.player2 = null;
                    break;
                case PlayerTestType.MarioAndLuigi:
                    OVManager.instance.player = player1;
                    OVManager.instance.player2 = player2;
                    break;
                case PlayerTestType.LuigiAndMario:
                    OVManager.instance.player = player2;
                    OVManager.instance.player2 = player1;
                    break;
                case PlayerTestType.GoombaFRFR:
                    OVManager.instance.player = Resources.Load<CharaSO>("Data/Players/Goomba");
                    OVManager.instance.player2 = Resources.Load<CharaSO>("Data/Players/Goomba");
                    break;
            }

        }
    }
}
