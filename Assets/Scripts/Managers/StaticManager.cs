using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticManager : MonoBehaviour
{
    public static StaticManager instance;

    public bool onBattle;
    public int battleEnteringCase;

    public bool marioAhead = false;

    public int battleAdvantageCase = 0;
    public bool company = false;

    public BattleSO goBattle;

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
        if (OVManager.instance.playerType == 2)
        {
            company = !OVManager.instance.secondaryPlayer.split;
        }
        else {
            company = false;
        }
    }
}
