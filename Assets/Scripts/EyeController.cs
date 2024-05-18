using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EyeController : MonoBehaviour
{
    public Material mat;
    public Transform handle;

    public Texture2D eyebig, eyemedium, eyesmall;
    public Transform handlesize;
    public Transform handleshake;

    public bool sizeOverride;
    public float sizeFactor;
    public float sizeSpeed;
    public void Update()
    {
        handle.localPosition = new Vector3(Mathf.Clamp(handle.localPosition.x, -1f, 1f), Mathf.Clamp(handle.localPosition.y, -1f, 1f), Mathf.Clamp(handle.localPosition.z, -1f, 1f));

        
        float randX = Random.Range(-1f, 1f);
        float randY = Random.Range(-1f, 1f);
        bool randomize = handleshake.transform.localPosition.y >= 0f;
        sizeOverride = randomize;

        Vector2 offset = new Vector2(randX * handleshake.transform.localPosition.y * 0.1f, randY * handleshake.transform.localPosition.y * 0.1f);
        if (!randomize) {
            offset = new Vector2(0f, 0f);
        }
        mat.SetTextureOffset("_BaseMap", new Vector2((handle.localPosition.x * 0.2f) + offset.x, (handle.localPosition.y * -0.2f) + offset.y));

        if (sizeOverride)
        {
            float sin = Mathf.Abs(Mathf.Sin(Time.time* sizeSpeed)) * sizeFactor;
            mat.SetTexture("_BaseMap", (sin > 0.75f) ? eyebig : (sin < 0.75f && sin > 0.25f) ? eyemedium : eyesmall);

        }
        else {
            mat.SetTexture("_BaseMap", (handlesize.transform.localPosition.y > 0.75f) ? eyebig : (handlesize.transform.localPosition.y < 0.75f && handlesize.transform.localPosition.y > 0.25f) ? eyemedium : eyesmall);

        }
    }
}
