using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuThing : MonoBehaviour
{

    public List<GameObject> gbs;



    void Update()
    {
        for (int i = 0; i < (int)PlayerTestType.GoombaFRFR+1; i++) {
            gbs[i].SetActive(i==(int)StaticManager.instance.playerTestType);
        }
    }
}
