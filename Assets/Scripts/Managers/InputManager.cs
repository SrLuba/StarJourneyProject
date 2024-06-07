using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public bool joystick = false;
    public int joystickID = 0;

    public bool aPress = false;
    public bool bPress = false;
    public bool xPress = false;
    public bool yPress = false;
    public bool aHold = false;
    public bool bHold = false;
    public bool xHold = false;
    public bool yHold = false;

    public bool lPress = false;
    public bool rPress = false;
    public bool lHold = false;
    public bool rHold = false;

    public bool startPress = false;
    public bool selectPress = false;
    public bool startHold = false;
    public bool selectHold = false;


    public Vector2 leftStick = new Vector2(0f, 0f);
    public Vector2 rightStick = new Vector2(0f, 0f);

    public AudioClip JoystickClip, KeyboardClip, DefaultErrorClip;

    public Animator animInputShower;



    public void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public void UpdateKeyboard() {
        if (joystick) return;
        // A
        aPress = Mouse.current.leftButton.wasPressedThisFrame;
        aHold = Mouse.current.leftButton.isPressed;
        
        // B
        bPress = Mouse.current.rightButton.wasPressedThisFrame;
        bHold = Mouse.current.rightButton.isPressed;
        // X
        xPress = Keyboard.current.spaceKey.wasPressedThisFrame;
        xHold = Keyboard.current.spaceKey.isPressed;
        // Y
        yPress = Keyboard.current.leftShiftKey.wasPressedThisFrame;
        yHold = Keyboard.current.leftShiftKey.isPressed;
        // L
        lPress = Keyboard.current.qKey.wasPressedThisFrame;
        lHold = Keyboard.current.qKey.isPressed;
        // R
        rPress = Keyboard.current.eKey.wasPressedThisFrame;
        rHold = Keyboard.current.eKey.isPressed;
        // START
        startPress = Keyboard.current.enterKey.wasPressedThisFrame;
        startHold = Keyboard.current.enterKey.isPressed;

        // SELECT
        selectPress = Keyboard.current.backspaceKey.wasPressedThisFrame;
        selectHold = Keyboard.current.backspaceKey.isPressed;

        float iX = ((Keyboard.current.dKey.isPressed) ? 1f : 0f) - ((Keyboard.current.aKey.isPressed) ? 1f : 0f);
        float iY = ((Keyboard.current.wKey.isPressed) ? 1f : 0f) - ((Keyboard.current.sKey.isPressed) ? 1f : 0f);

        leftStick = new Vector2(iX, iY).normalized;
        
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Stick
        float riX = Mouse.current.delta.ReadValue().x;
        float riY = Mouse.current.delta.ReadValue().y;

        rightStick = new Vector2(riX, riY);
    }

    public void UpdateJoystick() {
        if (!joystick) return;

        if (joystickID+1 > Gamepad.all.Count) {
            joystick = false;
            joystickID = 0;
            Debug.Log("<color=red>GAMEPAD DISCONNECTED OR ERROR | ENABLING KEYBOARD AGAIN</color>");
            if (DefaultErrorClip!=null) SoundManager.instance.Play(DefaultErrorClip);
            if (animInputShower != null) animInputShower.Play("JoystickError");
            return;
        }

        // A
        aPress = Gamepad.all[joystickID].buttonSouth.wasPressedThisFrame;
        aHold = Gamepad.all[joystickID].buttonSouth.isPressed;

        // X
        xPress = Gamepad.all[joystickID].buttonNorth.wasPressedThisFrame;
        xHold = Gamepad.all[joystickID].buttonNorth.isPressed;
        // Y
        yPress = Gamepad.all[joystickID].buttonWest.wasPressedThisFrame;
        yHold = Gamepad.all[joystickID].buttonWest.isPressed;

        // B
        bPress = Gamepad.all[joystickID].buttonEast.wasPressedThisFrame;
        bHold = Gamepad.all[joystickID].buttonEast.isPressed;

        // L
        lPress = Gamepad.all[joystickID].rightShoulder.wasPressedThisFrame;
        lHold = Gamepad.all[joystickID].rightShoulder.isPressed;

        // R
        rPress = Gamepad.all[joystickID].leftShoulder.wasPressedThisFrame;
        rHold = Gamepad.all[joystickID].leftShoulder.isPressed;

        // START
        startPress = Gamepad.all[joystickID].startButton.wasPressedThisFrame;
        startHold = Gamepad.all[joystickID].startButton.isPressed;

        // SELECT
        selectPress = Gamepad.all[joystickID].selectButton.wasPressedThisFrame;
        selectHold = Gamepad.all[joystickID].selectButton.isPressed;

        // Stick
        float iX = Gamepad.all[joystickID].leftStick.ReadValue().x;
        float iY = Gamepad.all[joystickID].leftStick.ReadValue().y;

        leftStick = new Vector2(iX, iY).normalized;
        // Stick
        float riX = Gamepad.all[joystickID].rightStick.ReadValue().x;
        float riY = Gamepad.all[joystickID].rightStick.ReadValue().y;

        rightStick = new Vector2(riX, riY).normalized;

    }

    public void JoystickLookUp() {
        if (joystick) return;

        if (Gamepad.all.Count > 0)
        {
            for (int i = 0; i < Gamepad.all.Count; i++)
            {
                if (Gamepad.all[i].startButton.isPressed)
                {
                    joystickID = i;
                    joystick = true;
                    Debug.Log("<color=green>FOUND GAMEPAD | " + Gamepad.all[i].displayName + "</color>");
                    if (animInputShower != null) animInputShower.Play("JoystickConnect");
                    if (JoystickClip != null) SoundManager.instance.Play(JoystickClip);
                }
            }
        }

    }

    public void KeyboardLookUp() {
        if (!joystick) return;

        if (Keyboard.current.anyKey.isPressed) {
            joystick = false;
            if (animInputShower != null) animInputShower.Play("KeyboardConnect");
            if (KeyboardClip != null) SoundManager.instance.Play(KeyboardClip);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (joystick) {
            UpdateJoystick();
            KeyboardLookUp();
        } else {
            UpdateKeyboard();
            JoystickLookUp();
        }

        if (Keyboard.current.f5Key.wasPressedThisFrame) UnityEngine.SceneManagement.SceneManager.LoadScene("Overworld");
    }
}
