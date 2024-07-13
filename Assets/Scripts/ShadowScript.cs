using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowScript : MonoBehaviour
{
    public Transform target;
    public ActorSO targetChara;

    public Vector3 Offset;

    public float minSize = 0.15f;
    public float maxSize = 2.0f;

    public float yOffset = 0f;
    public float floorY;

    public LayerMask floorMask;

    Transform child;
    public void Start()
    {
        child = new GameObject("t").transform;

        floorMask = targetChara.floorMask;
    }
    public void Update() {
        if (target == null) return;
        Adapt();
        child.position = target.position+ new Vector3(0f,.05f,0f);
        maxSize = targetChara.size;
        this.transform.position = new Vector3(target.position.x, floorY, target.position.z)+ targetChara.selfShadowOffset;

        float GSize = Mathf.Clamp(targetChara.size / (Mathf.Clamp(target.transform.position.y - floorY, 0f, 1000f)), minSize, maxSize);
        this.transform.localScale = Vector3.Lerp(this.transform.localScale, new Vector3(GSize, GSize, GSize), 15f*Time.deltaTime);

    }

    public void Adapt() {
        // Make the shadow adapt to surface normals
        Vector3 angle = new Vector3(0f, 0f, 0f);
       
        RaycastHit hit;
        if (Physics.Raycast(target.position + new Vector3(0f, 2f, 0f), Vector3.down, out hit, Mathf.Infinity, floorMask))
        {
            angle = Quaternion.FromToRotation(Vector3.up, hit.normal).eulerAngles;
            yOffset = hit.point.y+0.01f; 
        }
        else {
            angle = new Vector3(0f, 0f, 0f);
        }
        this.transform.eulerAngles = angle;
        //this.transform.eulerAngles = new Vector3(Mathf.LerpAngle(this.transform.eulerAngles.x,angle.x,25f*Time.deltaTime), 0f, Mathf.LerpAngle(this.transform.eulerAngles.z, angle.z, 25f * Time.deltaTime));
    }
}
