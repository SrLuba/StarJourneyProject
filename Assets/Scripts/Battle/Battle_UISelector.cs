using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_UISelector : MonoBehaviour
{
    public float value;
    public float valueScale;
    public float paddingmid;
    public float padding;
    public float valueAngle;
    public float valuesAngle;
    public float paddingAngle;
    public List<GameObject> gbs;

    public float animSpeed;
    public float animYPad;
    public float animYPadAng;
    public float animSpeedAng;

    public float secondaryPadding;

 
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < gbs.Count; i++) {
            gbs[i].transform.localPosition = Vector3.Lerp(gbs[i].transform.localPosition,  new Vector3(Mathf.Cos(value + (i* secondaryPadding)) * padding, Mathf.Sin(Time.time * animSpeed + i) * animYPad, Mathf.Sin(value + (i * secondaryPadding)) * padding), 15f*Time.deltaTime);
            gbs[i].transform.localScale =   Vector3.Lerp(gbs[i].transform.localScale, new Vector3( Mathf.Clamp( Mathf.Sin(value + (i* paddingmid)) * -valueScale,0.1f,2f) , Mathf.Clamp(Mathf.Sin(value + (i * paddingmid)) * -valueScale, 0.1f, 2f), Mathf.Clamp(Mathf.Sin(value + (i * paddingmid)) * -valueScale, 0.1f, 2f)), 15f*Time.deltaTime);
            gbs[i].transform.localEulerAngles = new Vector3(Mathf.Sin(Time.time * animSpeedAng + i) * animYPadAng, Mathf.LerpAngle(gbs[i].transform.localEulerAngles.y, valuesAngle+ Mathf.Cos(value + (i*valueAngle)) * paddingAngle, 15f*Time.deltaTime), gbs[i].transform.localEulerAngles.z);
        }
    }
}
