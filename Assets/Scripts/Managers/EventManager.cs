using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    public bool onEvent = false;

    public List<EventSO> events;

    public IEnumerator StartEvent(string identifier) {
        EventSO so = events.Find(x => x.name.ToUpper() == identifier.ToUpper());
        
        if (so!=null) {

            OVManager.instance.mainPlayer.canInput = true;
            OVManager.instance.mainPlayer.AI = false;

            if (OVManager.instance.secondaryPlayer != null)
            {
                OVManager.instance.secondaryPlayer.canInput = true;
            }
            OVManager.instance.mainCamera.ResetControl();

            yield return so.ev.doEvent();
        }
    }

    public void StartEventAsync(string identifier) {
        StartCoroutine(StartEvent(identifier));
    }
    
    public void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
    }
}
