using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MovementTypeCamera { 
    Instant,
    Smooth
} 
public class Battle_Camera : MonoBehaviour
{
    public static Battle_Camera instance;

    public Camera mainCamera;
    public Camera bgCamera;

    public MovementTypeCamera currentMovementType;
    public Vector3 vectorGoto;
    public CharaSO target;


    public Vector3 offset;
    public float speed;

    MusicSO music;

    Vector3 currentPositionTarget = Vector3.zero;

    public Vector3 defaultPosition;
    public float bpmEffectIntensity = 1f;
    float bpmEffect;

    public float currentFOV;
    public float defaultFOV = 60f;

    public Vector2 FOVClamp;
    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        music = MusicManager.instance.myMusic;
    }

    public void UpdateFOV() {

        float targetFOV = defaultFOV + bpmEffect;
        currentFOV = Mathf.Clamp(Mathf.Lerp(currentFOV, targetFOV, 8f*Time.deltaTime),FOVClamp.x, FOVClamp.y);

        this.mainCamera.fieldOfView = currentFOV;
        this.bgCamera.fieldOfView = currentFOV;
        bpmEffect = Mathf.Lerp(bpmEffect, 0f, 15f * Time.deltaTime);
    }
    public void Beat() {
        
        bpmEffect = bpmEffectIntensity;
    }
    public void UpdateSmooth() {
        if (currentMovementType != MovementTypeCamera.Smooth) return;

        Vector3 result = (target != null) ? target.selfBattle.getInstance(0, target.identifier).transform.position+vectorGoto + offset : defaultPosition;
        currentPositionTarget = result;

        this.transform.position = Vector3.Lerp(this.transform.position, currentPositionTarget, speed * Time.deltaTime);
    }
    
    public void Update()
    {
        UpdateSmooth();
        UpdateFOV();
    }
}
