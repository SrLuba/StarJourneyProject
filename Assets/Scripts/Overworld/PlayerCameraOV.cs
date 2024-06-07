using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraMode { 
    Default,
    Original
}
public class PlayerCameraOV : MonoBehaviour
{

    public CameraMode mode;
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
    public float angleOffsetY = 0f;

    public float angleOffsetX = 0f;


    public float xSpeedAngle = 0f;
    public float ySpeedAngle = 0f;



    public void ResetControl()
    {
        Mlock = false;
        disableSmooth = false;
        enableVectorOverride = false;
        linear = false;
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
        offset = (mode == CameraMode.Default) ? Vector3.Lerp(offset, new Vector3(0f, 4f, 0f), 15f*Time.deltaTime) : Vector3.Lerp(offset, new Vector3(0f, 2f, -4f), 15f * Time.deltaTime);
        CameraRotate();

        this.transform.position = linear ? Vector3.MoveTowards(this.transform.position, goVec + movOffset, speed * Time.deltaTime) : Vector3.Lerp(this.transform.position, goVec + movOffset, speed * Time.deltaTime);
        this.transform.eulerAngles = new Vector3(Mathf.LerpAngle(this.transform.eulerAngles.x, Mathf.Clamp(angleOffsetX + movOffset.z * cRotMultiplier, camXClamp.x, camXClamp.y), cRotSpeed * Time.deltaTime), Mathf.LerpAngle(this.transform.eulerAngles.y,  Mathf.Clamp(angleOffsetY + movOffset.x* cRotMultiplier, camYClamp.x, camYClamp.y), cRotSpeed * Time.deltaTime), this.transform.eulerAngles.z);
    }

    public void ChangeMode(CameraMode mode) { this.mode = mode; } 
    public void CameraTransitionateBTL() {
        camAnim.Play("Camera_A_BTLT");
        Mlock = true;
    }
    void Update()
    {
        target = linkedPlayer.transform;
    }
  
    public void CameraRotate() {
        if (mode == CameraMode.Default)
        {
            angleOffsetY += InputManager.instance.rightStick.x * ySpeedAngle;
            angleOffsetX += InputManager.instance.rightStick.y * xSpeedAngle;

            if (angleOffsetY >= 360f) angleOffsetY = 0f;

            if (angleOffsetX >= 90f) angleOffsetX = 90f;
            if (angleOffsetX <= -45f) angleOffsetX = -45f;
        }
        else if (mode == CameraMode.Original) {
            angleOffsetY = 0f;
            angleOffsetX = 0f;
        }
    }

    void FixedUpdate()
    {
        SmoothUpdate();
    }
}
