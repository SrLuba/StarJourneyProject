using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberIndividualDisplay : MonoBehaviour
{
    public List<float> yPositions; // Needs to be 10 or script won't work.
    public Transform StripP;

    public int number = 0;
    public float changeSpeed = 15f;

    public void Update()
    {
        StripP.transform.localPosition = Vector3.Lerp(StripP.transform.localPosition, new Vector3(0f, yPositions[number], 0f), changeSpeed * Time.deltaTime);
    }
}
