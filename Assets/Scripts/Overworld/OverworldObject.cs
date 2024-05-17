using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldObject : MonoBehaviour
{
    public Transform affectedObject;

    public float offset;
    public Vector3 visible;
    public float speed;

    Transform camera = null;

    void Start()
    {
        camera = Camera.main.transform;    
    }
    void FixedUpdate()
    {
        if (camera == null) return;

        bool v = camera.position.z < affectedObject.position.z + offset;

        affectedObject.transform.localScale = Vector3.Lerp(affectedObject.transform.localScale, v ? visible : new Vector3(0.001f, 0.001f, 0.001f), speed * Time.deltaTime);
    }
}
