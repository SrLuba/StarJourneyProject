using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ActionPanelVisual : MonoBehaviour
{
    public List<Transform> items;

    public List<float> itemsSel;

    public float padding = 1f;
    public float speed = 15f;
    public float factor = 1f;

    public Color Con, Coff;

    public int playerID = 0;

    public void UpdateItems(CharaSO so) { 
        
    }
    void Update()
    {
        int id = (playerID==0) ? OVActionManager.instance.actionID_Player : OVActionManager.instance.actionID_Adjacent;
        factor = itemsSel[id];
        
        for (int i = 0; i < items.Count; i++) {
            items[i].GetComponent<Image>().sprite = OVManager.instance.player.OVActor.perks[OVManager.instance.player.perkPackID].perks[i].icon;
   
            items[i].GetComponent<Image>().color = Color.Lerp(items[i].GetComponent<Image>().color, (i == id) ? Con : Coff, 15f*Time.deltaTime);
            items[i].localPosition = Vector3.Lerp(items[i].localPosition, new Vector3(Mathf.Cos(i+factor)*padding, Mathf.Sin(i + factor) * padding, 0f), speed*Time.deltaTime);
        }
    }
}
