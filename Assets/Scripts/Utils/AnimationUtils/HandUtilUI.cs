using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class HandUtilUI : MonoBehaviour
{
    public Transform target;
    public HandUtil HandUtil;
    public void Update()
    {
        target.localPosition = new Vector3(0f, Mathf.Clamp(target.localPosition.y, 0f, 1f),0f);
        HandUtil.testProgress = Mathf.Abs(100f*target.localPosition.y);
    }
}
