using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public enum EventItem_ActionType { 
    MoveAndRotate,
    Move,
    Rotate,
    SetPosition,
    SetRotation,
    Jump,
    Hammer,
    PlayAnimation,
    CameraLock,
    CameraUnlock,
    CameraSmoothEnable,
    CameraSmoothDisable,
    CameraVectorOverrideDisable,
    CameraVectorOverrideEnable,
    CameraLinearEnable,
    CameraLinearDisable,
    CameraResetControl
}
[System.Serializable]
public enum EventItem_Type
{
    Actor,
    Sound,
    Music,
    Camera,
    Object,
    Wait
}
    [System.Serializable]
public class EventItem
{
    public float delay = 0.1f;
    public EventItem_Type evType;
    public EventItem_ActionType evIType;
    public string targetActor;
    public string arguments;
    public bool async = false;

    public Vector2 vectorArgument;
    public Vector2 vector2Argument;
    public Vector3 vector3Argument;
    public float yArgument;
    public AudioClip audioArgument;

    public IEnumerator doItem() {
        yield return new WaitForSeconds(delay);
        if (evType == EventItem_Type.Actor)
        {
            PlayerOV g = GameObject.Find("OVACTOR_" + targetActor.ToUpper()).GetComponent<PlayerOV>();
            Debug.LogWarning("EVENT | FIND " + "OVACTOR_" + targetActor.ToUpper());
          
            Debug.LogWarning("EVENT | ACTION " + "OVACTOR_" + targetActor.ToUpper() + " " + evIType.ToString());
            if (g != null)
            {
                if (!async)
                {
                    yield return g.GetComponent<OVGenericActor>().EVAction(this.evIType, this.vectorArgument, this.vector2Argument, this.arguments);
                }
                else {
                    g.GetComponent<OVGenericActor>().StartCoroutine(g.GetComponent<OVGenericActor>().EVAction(this.evIType, this.vectorArgument, this.vector2Argument, this.arguments));
                }
            }
        }
        else if (evType == EventItem_Type.Camera)
        {
            if (evIType == EventItem_ActionType.CameraLock)
            {
                OVManager.instance.mainCamera.Mlock = true;
            }
            else if (evIType == EventItem_ActionType.CameraUnlock)
            {
                OVManager.instance.mainCamera.Mlock = false;
            }
            else if (evIType == EventItem_ActionType.CameraSmoothEnable)
            {
                OVManager.instance.mainCamera.disableSmooth = false;
            }
            else if (evIType == EventItem_ActionType.CameraSmoothDisable)
            {
                OVManager.instance.mainCamera.disableSmooth = true;
            }
            else if (evIType == EventItem_ActionType.SetPosition)
            {
                OVManager.instance.mainCamera.Mlock = true;
                OVManager.instance.mainCamera.transform.position = vectorArgument;
            }
            else if (evIType == EventItem_ActionType.SetRotation)
            {
                OVManager.instance.mainCamera.Mlock = true;
                OVManager.instance.mainCamera.transform.eulerAngles = vectorArgument;
            }
            else if (evIType == EventItem_ActionType.CameraVectorOverrideEnable)
            {
                OVManager.instance.mainCamera.ResetControl();
                OVManager.instance.mainCamera.vectorOverride = vector3Argument;
                OVManager.instance.mainCamera.enableVectorOverride = true;
            }
            else if (evIType == EventItem_ActionType.CameraVectorOverrideDisable)
            {
                OVManager.instance.mainCamera.ResetControl();
                OVManager.instance.mainCamera.vectorOverride = vector3Argument;
                OVManager.instance.mainCamera.enableVectorOverride = false;
            }
            else if (evIType == EventItem_ActionType.CameraLinearEnable)
            {
                OVManager.instance.mainCamera.linear = true;
            }
            else if (evIType == EventItem_ActionType.CameraLinearDisable)
            {
                OVManager.instance.mainCamera.linear = false;
            }
            else if (evIType == EventItem_ActionType.CameraResetControl)
            {
                OVManager.instance.mainCamera.ResetControl();
            }
            else if (evIType == EventItem_ActionType.Move)
            {
                OVManager.instance.mainCamera.vectorOverride = vector3Argument;
            }
         
        }
        else if (evType == EventItem_Type.Object)
        {
            GameObject g = GameObject.Find(targetActor);
            if (g != null) yield return g.GetComponent<OVGenericActor>().EVAction(this.evIType, this.vectorArgument, this.vector2Argument, this.arguments);
        }
        else if (evType == EventItem_Type.Music)
        {
            MusicManager.instance.PlayClip(this.audioArgument, true);
        }
        if (audioArgument!=null) SoundManager.instance.Play(this.audioArgument);
        
    }
}

[System.Serializable]
public class EventInfo {
    public string eventIdentifier;
    public string eventName;
    public List<EventItem> evIts;

    public IEnumerator doEvent() {
        EventManager.instance.onEvent = true;

        Debug.LogWarning("EVENT START | " + eventName + "|" + eventIdentifier);
        for (int i = 0; i < evIts.Count; i++) {
            Debug.LogWarning("EVENT ITEM"+i.ToString()+" | " + eventName + "|" + eventIdentifier);
            yield return evIts[i].doItem();
            yield return new WaitForSeconds(0.1f);
        }
        Debug.LogWarning("EVENT END | " + eventName + "|" + eventIdentifier);

        EventManager.instance.onEvent = false;
    }
}


[CreateAssetMenu]
public class EventSO : ScriptableObject
{
    public EventInfo ev;
}
