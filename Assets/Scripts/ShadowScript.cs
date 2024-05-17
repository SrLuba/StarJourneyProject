using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowScript : MonoBehaviour
{
    public Transform target;
    public CharaSO targetChara;

    public Vector3 Offset;

    public float minSize = 0.15f;
    public float maxSize = 2.0f;

    public float floorY;

    public void Update() {
        if (target == null) return;
        maxSize = targetChara.size;
        this.transform.position = new Vector3(target.position.x, floorY, target.position.z)+ targetChara.selfShadowOffset;

        float GSize = Mathf.Clamp(targetChara.size / (Mathf.Clamp(target.transform.position.y - floorY, 0f, 1000f)), minSize, maxSize);
        this.transform.localScale = Vector3.Lerp(this.transform.localScale, new Vector3(GSize, GSize, GSize), 15f*Time.deltaTime);
    }
}
