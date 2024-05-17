using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraOV : MonoBehaviour
{
    public float speed;
    public PlayerOV linkedPlayer;

    public Vector3 offset;
    public Vector3 goVec;
    public Vector3 movOffset;

    public Vector2 camYClamp;
    public Vector2 camXClamp;
    public float cRotSpeed;
    public float cRotMultiplier;
    

    public Transform target;
    public Animator camAnim;

    public float inputEffect = 1f;

    public Vector3 vectorOverride = new Vector3(0f, 0f, 0f);
    public bool enableVectorOverride = false;

    public bool linear = false;

    public bool Mlock = false;
    public bool disableSmooth = false;

    public void ResetControl()
    {
        Mlock = false;
        disableSmooth = false;
    }
    public void InstantUpdate() {
        goVec = target.position + offset;
        movOffset = new Vector3(linkedPlayer.input.x * inputEffect, 0f, linkedPlayer.input.y * inputEffect);
        this.transform.position = goVec + movOffset;
    }
    public void SmoothUpdate() {
        if (Mlock) return;
        if (disableSmooth) {
           InstantUpdate();
           return;
        }
       

        goVec = target.position + offset;
        movOffset = new Vector3(linkedPlayer.input.x * inputEffect, 0f, linkedPlayer.input.y * inputEffect);

        if (enableVectorOverride)
        {
            goVec = vectorOverride + offset;
            movOffset = new Vector3(0f, 0f, 0f);
        }

        
        this.transform.position = linear ? Vector3.MoveTowards(this.transform.position, goVec + movOffset, speed * Time.deltaTime) : Vector3.Lerp(this.transform.position, goVec + movOffset, speed * Time.deltaTime);
        this.transform.eulerAngles = new Vector3(Mathf.LerpAngle(this.transform.eulerAngles.x, Mathf.Clamp(movOffset.z * cRotMultiplier, camXClamp.x, camXClamp.y), cRotSpeed * Time.deltaTime), Mathf.LerpAngle(this.transform.eulerAngles.y, Mathf.Clamp(movOffset.x* cRotMultiplier, camYClamp.x, camYClamp.y), cRotSpeed * Time.deltaTime), this.transform.eulerAngles.z);
    }
    public void CameraTransitionateBTL() {
        camAnim.Play("Camera_A_BTLT");
        Mlock = true;
    }
    void Update()
    {
        target = linkedPlayer.transform;
    }
    void FixedUpdate()
    {
        SmoothUpdate();
    }
}
