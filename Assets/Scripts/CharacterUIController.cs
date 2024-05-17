using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUIController : MonoBehaviour
{
    public GameObject SplitP, CompanyP;
    public GameObject SplitPM, SplitPL;


    void Update()
    {
        if (OVManager.instance == null) return;
       
        if (OVManager.instance.mainPlayer == null) return;
        if (OVManager.instance.secondaryPlayer == null)
        {
            CompanyP.SetActive(false);
            SplitP.SetActive(true);
            SplitPM.SetActive(OVManager.instance.mainPlayer.selfChara.identifier.ToUpper() == "MARIO");
            SplitPL.SetActive(OVManager.instance.mainPlayer.selfChara.identifier.ToUpper() == "LUIGI");
            this.enabled = false;
            return;
        }
        CompanyP.SetActive(!OVManager.instance.secondaryPlayer.split);
        SplitP.SetActive(OVManager.instance.secondaryPlayer.split);
        SplitPM.SetActive(OVManager.instance.mainPlayer.selfChara.identifier.ToUpper() == "MARIO");
        SplitPL.SetActive(OVManager.instance.mainPlayer.selfChara.identifier.ToUpper() == "LUIGI");
    }
}
