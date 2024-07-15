using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public BattleActorSO target;


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

    public bool camOverride = false;
    public Vector3 camOverridePosition, camOverrideAngle;
    public Vector3 camOverrideOffset;
    
    public float camOverrideFOV;
    public Transform dummy;
    public bool active = true;

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
        currentFOV = Mathf.Clamp(Mathf.Lerp(currentFOV, targetFOV, 4f*Time.deltaTime),FOVClamp.x, FOVClamp.y);

        this.mainCamera.fieldOfView = currentFOV;
        this.bgCamera.fieldOfView = currentFOV;
        bpmEffect = Mathf.Lerp(bpmEffect, 0f, 15f * Time.deltaTime);
    }
    public void Beat() {
        if (!BattleManager.instance.cameraBPM) return;
        bpmEffect = bpmEffectIntensity; 
    }
    public void UpdateSmooth() {
        if (!active || camOverride) return;
        if (currentMovementType != MovementTypeCamera.Smooth) return;

        Vector3 result = (target != null) ? new Vector3(target.getInstance().transform.position.x, 0f, target.getInstance().transform.position.z) +vectorGoto + offset : defaultPosition;
        
        if (!camOverride) {
            currentPositionTarget = result;
        }

        this.transform.position = Vector3.Lerp(this.transform.position, currentPositionTarget, speed * Time.deltaTime);
        this.transform.eulerAngles = new Vector3(
     Mathf.LerpAngle(this.transform.eulerAngles.x, 0f, speed * Time.deltaTime),
     Mathf.LerpAngle(this.transform.eulerAngles.y, 0f, speed * Time.deltaTime),
     Mathf.LerpAngle(this.transform.eulerAngles.z, 0f, speed * Time.deltaTime));
    }

    public void UpdateOverride() {
        if (!camOverride) return;

        this.transform.position = Vector3.Lerp(this.transform.position, camOverridePosition+ camOverrideOffset, speed * Time.deltaTime);

        if (target == null)
        {
            this.transform.eulerAngles = new Vector3(
           Mathf.LerpAngle(this.transform.eulerAngles.x, this.camOverrideAngle.x, speed * Time.deltaTime),
           Mathf.LerpAngle(this.transform.eulerAngles.y, this.camOverrideAngle.y, speed * Time.deltaTime),
           Mathf.LerpAngle(this.transform.eulerAngles.z, this.camOverrideAngle.z, speed * Time.deltaTime));
        }
        else {
            this.transform.eulerAngles = new Vector3(
               Mathf.LerpAngle(this.transform.eulerAngles.x, this.dummy.eulerAngles.x, speed * Time.deltaTime),
               Mathf.LerpAngle(this.transform.eulerAngles.y, this.dummy.eulerAngles.y, speed * Time.deltaTime),
               Mathf.LerpAngle(this.transform.eulerAngles.z, this.dummy.eulerAngles.z, speed * Time.deltaTime));
        }
       
        this.mainCamera.fieldOfView = Mathf.Lerp(this.mainCamera.fieldOfView, camOverrideFOV, speed * Time.deltaTime);
        this.bgCamera.fieldOfView = Mathf.Lerp(this.bgCamera.fieldOfView, camOverrideFOV, speed * Time.deltaTime);
    }
    
    public void Update()
    {
        if (BattleManager.instance.victory) {
            this.mainCamera.fieldOfView = Mathf.Lerp(this.mainCamera.fieldOfView, 60f, speed * Time.deltaTime);
            this.bgCamera.fieldOfView = Mathf.Lerp(this.bgCamera.fieldOfView, 60f, speed * Time.deltaTime);
            return;
        }

        dummy.transform.position = this.transform.position;
        if (target!=null) dummy.LookAt(target.getInstance().transform);

        UpdateSmooth();
        UpdateFOV();
        UpdateOverride();
    }
}
