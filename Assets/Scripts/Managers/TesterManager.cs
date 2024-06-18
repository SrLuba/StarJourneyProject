using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TesterManager : MonoBehaviour
{
    public List<TMP_Text> text;
    public List<string> inputType;

    public Transform Lstick;
    public float LstickSize;

    public Transform Rstick;
    public float RstickSize;

    public TMP_Text programCounterTXT;
    public TMP_Text fpsTXT;

    float timer = 0f;

    int fps = 0;
    int programCounter = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public Color getInput(string type) {
        Color result = new Color(1f, 1f, 1f, 1f);
        Color pressedColor = new Color(0f, 0f, 0f, 1f); 
        switch (type.ToUpper()) {
            case "A":
                if (InputManager.instance.aHold) { result = pressedColor; }
                break;
            case "B":
                if (InputManager.instance.bHold) { result = pressedColor; }
                break;
            case "X":
                if (InputManager.instance.xHold) { result = pressedColor; }
                break;
            case "Y":
                if (InputManager.instance.yHold) { result = pressedColor; }
                break;
            case "R":
                if (InputManager.instance.rHold) { result = pressedColor; }
                break;
            case "L":
                if (InputManager.instance.lHold) { result = pressedColor; }
                break;
            case "START":
                if (InputManager.instance.startHold) { result = pressedColor; }
                break;
            case "SELECT":
                if (InputManager.instance.selectHold) { result = pressedColor; }
                break;
        }
        return result;
    }
    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        if (timer >= 1f) {
            fpsTXT.text = "FPS - " + fps.ToString();

            fps = 0;
            timer = 0f;
        }
    }
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < text.Count; i++) {
            text[i].color = this.getInput(inputType[i]);
        }

    
        Lstick.transform.localPosition = new Vector3(InputManager.instance.leftStick.normalized.x * LstickSize, InputManager.instance.leftStick.normalized.y * LstickSize, Lstick.transform.position.z);
        Rstick.transform.localPosition = new Vector3(InputManager.instance.rightStick.normalized.x * RstickSize, InputManager.instance.rightStick.normalized.y * RstickSize, Rstick.transform.position.z);
        programCounter++;
        fps++;
        
        programCounterTXT.text = "Program Counter - " + programCounter.ToString();
    }
}
