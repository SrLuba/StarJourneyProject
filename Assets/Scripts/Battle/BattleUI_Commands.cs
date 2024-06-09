using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUI_Commands : MonoBehaviour
{
    public static BattleUI_Commands instance;
    public Animator p1, p2;
    public List<Animator> p_types;
    public List<GameObject> types;

    int pCount = 0;

    public List<GameObject> statusMeters;


    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        Setup_Player_UI();
       
    }
    public void Update_Player_UI(int type) {
        if (type == -1) {
            if (p1 != null) p1.Play("Idle");
            if (p2 != null) p2.Play("Idle");
            return;
        }

        if (p1 != null) {
            p1.Play("P1_" + ((type == 0) ? "JUMP" : "HAMMER"));
        }

        if (p2 != null) {
            p2.Play("P2_" + ((type == 0) ? "JUMP" : "HAMMER"));
        }
    }
    public void Setup_Player_UI() {
        types[0].SetActive((StaticManager.instance.players.Count==1));
        types[1].SetActive((StaticManager.instance.players.Count==2));
        types[2].SetActive((StaticManager.instance.players.Count>=3));

        pCount = StaticManager.instance.players.Count;

        statusMeters[0].SetActive((StaticManager.instance.players.Count >= 1));
        statusMeters[1].SetActive((StaticManager.instance.players.Count >= 2));
        statusMeters[2].SetActive((StaticManager.instance.players.Count >= 3));

        if (StaticManager.instance.players.Count == 1) {
            p1 = p_types[0];
            p2 = null;
            
        }
        else if(StaticManager.instance.players.Count == 2) {
            p1 = p_types[1];
            p2 = p_types[2];
        }
        else if (StaticManager.instance.players.Count == 3)
        {
            p1 = p_types[3];
            p2 = p_types[4];
        }
    }
}
