using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OVGenericActor : MonoBehaviour
{

    public Animator anim;

    public PlayerOV selfOV;

    public IEnumerator EVAction(EventItem_ActionType evType, Vector2 vectorArg, Vector2 vectorSecondArg, string argument) 
    {
        Debug.LogWarning("EVENT | SET INPUT FALSE");
        selfOV.canInput = false;
        yield return new WaitForSeconds(0.1f);
        Debug.LogWarning("EVENT | CHECKING EVENT TYPE");
        if (evType == EventItem_ActionType.MoveAndRotate)
        {
            Debug.LogWarning("EVENT | MOVE AND ROTATE");
            float par = float.Parse(argument);
            Debug.LogWarning("EVENT | PLAYEROV | SET SETTING AI ON");
            selfOV.AI = true;
            bool ogft = selfOV.followTransform;
            selfOV.followTransform = false;
            selfOV.followVector = new Vector3(vectorArg.x, selfOV.gameObject.transform.position.y, vectorArg.y);
            Debug.LogWarning("EVENT | PLAYEROV | SET SETTING AI ON STEP 2");
            while (Vector3.Distance(selfOV.gameObject.transform.position, selfOV.followVector) > 0.5f)
            {
                selfOV.AI = true;
                selfOV.followTransform = false;
                selfOV.followVector = new Vector3(vectorArg.x, selfOV.gameObject.transform.position.y, vectorArg.y);
                Debug.Log("EVENT | PLAYEROV | GOING FOR POINT");
                yield return new WaitForSeconds(0.1f);
            }
            Debug.LogWarning("EVENT | PLAYEROV | ENDED PATH GO");
            yield return new WaitForSeconds(0.1f);
            Debug.LogWarning("EVENT | PLAYEROV | SET SETTING AI OFF");
            selfOV.AI = false;
            selfOV.followTransform = ogft;

            selfOV.animV = new Vector2((float)Mathf.Sin(par*Mathf.Deg2Rad), (float)Mathf.Cos(par * Mathf.Deg2Rad));
        }
        else if (evType == EventItem_ActionType.Jump)
        {
            selfOV.Jump();
        }
        else if (evType == EventItem_ActionType.Rotate)
        {
            float par = float.Parse(argument);
            selfOV.animV = new Vector2((float)Mathf.Sin(par * Mathf.Deg2Rad), (float)Mathf.Cos(par * Mathf.Deg2Rad));
        }
        else if (evType == EventItem_ActionType.Hammer)
        {
            selfOV.DoPerk(1);
        }
        else if (evType == EventItem_ActionType.Move)
        {
            Debug.LogWarning("EVENT | MOVE ONLY");
            float par = float.Parse(argument);
            Debug.LogWarning("EVENT | PLAYEROV | SET SETTING AI ON");
            selfOV.AI = true;
            bool ogft = selfOV.followTransform;
            selfOV.followTransform = false;
            selfOV.followVector = new Vector3(vectorArg.x, selfOV.gameObject.transform.position.y, vectorArg.y);
            Debug.LogWarning("EVENT | PLAYEROV | SET SETTING AI ON STEP 2");
            while (Vector3.Distance(selfOV.gameObject.transform.position, selfOV.followVector) > 0.5f)
            {
                Debug.Log("EVENT | PLAYEROV | GOING FOR POINT");
                yield return new WaitForSeconds(0.1f);
            }
            Debug.LogWarning("EVENT | PLAYEROV | ENDED PATH GO");
            yield return new WaitForSeconds(0.1f);
            Debug.LogWarning("EVENT | PLAYEROV | SET SETTING AI OFF");
            selfOV.AI = false;
            selfOV.followTransform = ogft;
        }
        else if (evType == EventItem_ActionType.PlayAnimation) {
            selfOV.animationOverride = true;
            selfOV.anim.Play(argument);
            yield return new WaitForSeconds(selfOV.anim.GetCurrentAnimatorClipInfo(0)[0].clip.length);
            selfOV.animationOverride = false;
        }
        selfOV.canInput = true;
        Debug.LogWarning("EVENT | END ITEM");
    }
}
