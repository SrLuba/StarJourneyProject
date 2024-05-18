using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* - TriggerManager -
    Handles Trigger Events, Those are the events that happen when you press a button, or do something important that changes something in the level
 */
public enum TriggerEventType { 
    CameraLinearDrag,
    CameraSmoothDrag,
    FadeInAndOut
}
public enum TriggerStepType
{
    ObjectMove,
    ObjectRotate,
    ObjectDestroy,
    ObjectAnim
}
[System.Serializable]
public class TriggerEventStep
{
    public TriggerStepType type;

    public string seekName;

    public float fArg;
    public string strArg;
    public Vector2 vec2Arg;
    public Vector3 vec3Arg;
    // 
    public IEnumerator doStep() {
        GameObject target = GameObject.Find(seekName);
        if (type == TriggerStepType.ObjectMove)
        {

            while (Vector3.Distance(target.transform.position, vec3Arg) > 0.01f)
            {
                target.transform.position = Vector3.MoveTowards(target.transform.position, vec3Arg, fArg * Time.deltaTime);
                yield return new WaitForSeconds(0.01f);
            }

        }
        else if (type == TriggerStepType.ObjectRotate)
        {
            while (target.transform.eulerAngles != vec3Arg)
            {
                target.transform.eulerAngles = new Vector3(Mathf.MoveTowardsAngle(target.transform.eulerAngles.x, vec3Arg.x, fArg * Time.deltaTime), Mathf.MoveTowardsAngle(target.transform.eulerAngles.y, vec3Arg.y, fArg * Time.deltaTime), Mathf.MoveTowardsAngle(target.transform.eulerAngles.z, vec3Arg.z, fArg * Time.deltaTime));
                yield return new WaitForSeconds(0.01f);
            }
        }
        else if (type == TriggerStepType.ObjectDestroy)
        {
            MonoBehaviour.Destroy(target);
        }
        else if (type == TriggerStepType.ObjectAnim) {
            target.GetComponent<Animator>().Play(strArg);
            yield return new WaitForSeconds(target.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length);
        }
        yield return new WaitForSeconds(.1f);
    }
}
[System.Serializable]
public class TriggerEvent {
    public string identifier;
    public TriggerEventType type;

    public AudioClip triggerEventSound;
    public List<TriggerEventStep> steps;
    public Vector3 cameraTargetPosition;

    public IEnumerator doEvent() {
        EventManager.instance.onEvent = true;

        OVManager.instance.mainPlayer.canInput = false;
        OVManager.instance.mainPlayer.input = new Vector2(0f, 0f);

        if (OVManager.instance.secondaryPlayer != null)
        {
            OVManager.instance.secondaryPlayer.canInput = false;
            OVManager.instance.secondaryPlayer.input = new Vector2(0f, 0f);
        }

        OVManager.instance.mainCamera.ResetControl();

        OVManager.instance.mainCamera.linear = (type == TriggerEventType.CameraLinearDrag);
        OVManager.instance.mainCamera.disableSmooth = (type == TriggerEventType.CameraSmoothDrag);
        OVManager.instance.mainCamera.enableVectorOverride = true;
        OVManager.instance.mainCamera.vectorOverride = cameraTargetPosition;

        if (type == TriggerEventType.FadeInAndOut) OVManager.instance.mainCamera.InstantUpdate();
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < steps.Count; i++) yield return steps[i].doStep();

        yield return new WaitForSeconds(1f);
        
        EventManager.instance.onEvent = false;
        OVManager.instance.mainPlayer.canInput = true;
        OVManager.instance.mainPlayer.AI = false;

        if (OVManager.instance.secondaryPlayer != null)
        {
            OVManager.instance.secondaryPlayer.canInput = true;
            OVManager.instance.secondaryPlayer.AI = true;
        }
        OVManager.instance.mainCamera.enableVectorOverride = false;
        OVManager.instance.mainCamera.linear = false;
        OVManager.instance.mainCamera.disableSmooth = false;
        OVManager.instance.mainCamera.ResetControl();
    }

}
public class TriggerManager : MonoBehaviour
{
    public static TriggerManager instance;

    public List<TriggerEvent> triggerEvents;

    public void Awake()
    {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        }

        instance = this;
    }

    public bool TriggerButton(string identifier) {
        TriggerEvent te = triggerEvents.Find(x => x.identifier.ToUpper() == identifier.ToUpper());

        if (te == null) return false;

        StartCoroutine(te.doEvent());
        return true;
    }
}
