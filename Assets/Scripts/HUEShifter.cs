using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class HUEShifter : MonoBehaviour
{
    public Volume volume;
    private ColorAdjustments colorAdjustments;
    private VolumeParameter<float> hueShift = new VolumeParameter<float>();
    public float timeScale = 1f;

    // Start is called before the first frame update
    void Start()
    {
        volume.profile.TryGet<ColorAdjustments>(out colorAdjustments);
    }
    bool end = false;
    public void End() {
        hueShift.value = 0f;
        end = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (colorAdjustments == null)
            return;

        if (end)
        {
            hueShift.value = 0f;
            colorAdjustments.hueShift.SetValue(hueShift);
            return;
        }

        hueShift.value += Time.deltaTime * timeScale;
        if (hueShift.value >= 180f) hueShift.value = -180f;

        colorAdjustments.hueShift.SetValue(hueShift);
    }
}
