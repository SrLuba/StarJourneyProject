using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class UTIL_StateCoordinateDefine {
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;

    public UTIL_StateCoordinateDefine(Vector3 position, Vector3 rotation, Vector3 scale) { 
        this.position = position; this.rotation = rotation; this.scale = scale;
    }
    
}
[System.Serializable]
public class ANIMUTIL_FINGER_POSDEFINE
{
    public UTIL_StateCoordinateDefine positionForStart;
    public UTIL_StateCoordinateDefine positionForMiddle;
    public UTIL_StateCoordinateDefine positionForEnd;

    public ANIMUTIL_FINGER_POSDEFINE() { }
}
[System.Serializable]
public class ANIMUTIL_FINGER_PROGRESSED_POSDEFINE
{

    public ANIMUTIL_FINGER_POSDEFINE start, end;

    public ANIMUTIL_FINGER_POSDEFINE getProgress(float progress) {
        ANIMUTIL_FINGER_POSDEFINE result = new ANIMUTIL_FINGER_POSDEFINE();

        float finalProgress = Mathf.Clamp(progress, 0f, 100f);

        Vector3 startFingerPosition = Vector3.MoveTowards(start.positionForStart.position, end.positionForStart.position, progress);
        Vector3 startFingerRotation = Vector3.MoveTowards(start.positionForStart.rotation, end.positionForStart.rotation, progress); 
        Vector3 startFingerScale = new Vector3(1f, 1f, 1f);

        Vector3 middleFingerPosition = Vector3.MoveTowards(start.positionForMiddle.position, end.positionForMiddle.position, progress);
        Vector3 middleFingerRotation = Vector3.MoveTowards(start.positionForMiddle.rotation, end.positionForMiddle.rotation, progress);
        Vector3 middleFingerScale = new Vector3(1f, 1f, 1f);

        Vector3 endFingerPosition = Vector3.MoveTowards(start.positionForEnd.position, end.positionForEnd.position, progress);
        Vector3 endFingerRotation = Vector3.MoveTowards(start.positionForEnd.rotation, end.positionForEnd.rotation, progress);
        Vector3 endFingerScale = new Vector3(1f, 1f, 1f);

        UTIL_StateCoordinateDefine resultStart = new UTIL_StateCoordinateDefine(startFingerPosition, startFingerRotation, startFingerScale);
        UTIL_StateCoordinateDefine resultMiddle = new UTIL_StateCoordinateDefine(middleFingerPosition, middleFingerRotation, middleFingerScale);
        UTIL_StateCoordinateDefine resultEnd = new UTIL_StateCoordinateDefine(endFingerPosition, endFingerRotation, endFingerScale);

        result.positionForStart = resultStart;
        result.positionForMiddle = resultMiddle;
        result.positionForEnd = resultEnd;

        return result;
    }

}

[System.Serializable]
public class ANIMUTIL_FINGER {
    public Transform start;
    public Transform middle;
    public Transform end;


    public ANIMUTIL_FINGER_PROGRESSED_POSDEFINE closeState;

    public void Store(int type) {
        ANIMUTIL_FINGER_POSDEFINE result = new ANIMUTIL_FINGER_POSDEFINE();


        UTIL_StateCoordinateDefine resultStart = new UTIL_StateCoordinateDefine(start.localPosition, start.localEulerAngles, start.localScale);
        UTIL_StateCoordinateDefine resultMiddle = new UTIL_StateCoordinateDefine(middle.localPosition, middle.localEulerAngles, middle.localScale);
        UTIL_StateCoordinateDefine resultEnd = new UTIL_StateCoordinateDefine(end.localPosition, end.localEulerAngles, end.localScale);
        result.positionForStart = resultStart;
        result.positionForMiddle = resultMiddle;
        result.positionForEnd = resultEnd;


        if (type == 0)
        {
            closeState.start = result;

        }
        else {
            closeState.end = result;
        }
    }
    public void Set(float progress)
    {
        ANIMUTIL_FINGER_POSDEFINE result = closeState.getProgress(progress);

        if (this.start != null)
        {
            this.start.localPosition = result.positionForStart.position;
            this.start.localEulerAngles = result.positionForStart.rotation;
        }
        if (this.middle != null)
        {
            this.middle.localPosition = result.positionForMiddle.position;
            this.middle.localEulerAngles = result.positionForMiddle.rotation;
        }
        if (this.end != null)
        {
            this.end.localPosition = result.positionForEnd.position;
            this.end.localEulerAngles = result.positionForEnd.rotation;
        }
    }
}
[ExecuteInEditMode]
public class HandUtil : MonoBehaviour
{

    public ANIMUTIL_FINGER index, middle, pinky, ring, thumb;
    public float testProgress = 0f;

    public bool updatePosition = false;
    public int editorValue = 0;
    public bool storeValue = true;

    void Update()
    {
        if (storeValue) {
            index.Store(editorValue);
            middle.Store(editorValue);
            pinky.Store(editorValue);
            ring.Store(editorValue);
            thumb.Store(editorValue);

            storeValue = false;
            return;
        }

        if (updatePosition) { 
            testProgress = Mathf.Clamp(testProgress, 0f, 100f);
            index.Set(testProgress);
            middle.Set(testProgress);
            pinky.Set(testProgress);
            ring.Set(testProgress);
            thumb.Set(testProgress);

        }
    }
}
