using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberDisplayer : MonoBehaviour
{
    public int maxValue = 99;
    public List<NumberIndividualDisplay> displays;

    public int number = 99;

    public List<Vector2> offsets;
    public List<Vector2> sizes;

    int currentNumber = 0;
    void Update()
    {
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        if (currentNumber == number) return;

        currentNumber = number;
        currentNumber = Mathf.Clamp(currentNumber, 0, maxValue);
        char[] chars = currentNumber.ToString().ToCharArray();

       // this.transform.localPosition = Vector3.zero;


       // this.transform.localPosition = new Vector3(offsets[chars.Length - 1].x, offsets[chars.Length - 1].y, this.transform.localPosition.z); this.transform.localPosition = new Vector3(offsets[chars.Length - 1].x, offsets[chars.Length - 1].y, this.transform.localPosition.z);
        this.transform.localScale = new Vector3(sizes[chars.Length - 1].x, sizes[chars.Length - 1].y, this.transform.localScale.z);

        for (int i = 0; i < displays.Count; i++) {

            displays[i].gameObject.SetActive(!(i >= chars.Length));

            if ((i <= chars.Length-1)) { 
                int num = int.Parse(chars[i].ToString());
                displays[i].number = num;
            }
        }   

    }
}
