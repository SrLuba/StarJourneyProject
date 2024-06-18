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

    public bool dpadDownPress = false;
    public bool dpadUpPress = false;
    public bool dpadRightPress = false;
    public bool dpadLeftPress = false;


    public string aBinding = "MOUSE_L";
    public string bBinding = "MOUSE_R";
    public string yBinding = "KEY_SPACE";
    public string xBinding = "KEY_LEFT_SHIFT";


    public bool swapLR = false;


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
        aPress = this.getBinding(aBinding).wasPressedThisFrame;
        aHold = this.getBinding(aBinding).isPressed;
        // B
        bPress = this.getBinding(bBinding).wasPressedThisFrame;
        bHold = this.getBinding(bBinding).isPressed;
        // X
        xPress = this.getBinding(xBinding).wasPressedThisFrame;
        xHold = this.getBinding(xBinding).isPressed;
        // Y
        yPress = this.getBinding(yBinding).wasPressedThisFrame;
        yHold = this.getBinding(yBinding).isPressed;

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


        dpadDownPress = Keyboard.current.downArrowKey.wasPressedThisFrame;
        dpadLeftPress = Keyboard.current.leftArrowKey.wasPressedThisFrame;
        dpadRightPress = Keyboard.current.rightArrowKey.wasPressedThisFrame;
        dpadUpPress = Keyboard.current.upArrowKey.wasPressedThisFrame;

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

    public UnityEngine.InputSystem.Controls.ButtonControl getBinding(string id) {

        UnityEngine.InputSystem.Controls.ButtonControl result = Keyboard.current.aKey;
        if (joystick)
        {

            switch (id.ToUpper())
            {
                case "BTN_A":
                    result = Gamepad.all[joystickID].aButton;
                    break;
                case "BTN_X":
                    result = Gamepad.all[joystickID].xButton;
                    break;
                case "BTN_Y":
                    result = Gamepad.all[joystickID].yButton;
                    break;
                case "BTN_B":
                    result = Gamepad.all[joystickID].bButton;
                    break;
            }

            }
        else {
            switch (id.ToUpper())
            {
                case "KEY_Q":
                    result = Keyboard.current.qKey;
                    break;
                case "KEY_W":
                    result = Keyboard.current.wKey;
                    break;
                case "KEY_E":
                    result = Keyboard.current.eKey;
                    break;
                case "KEY_R":
                    result = Keyboard.current.rKey;
                    break;
                case "KEY_T":
                    result = Keyboard.current.tKey;
                    break;
                case "KEY_Y":
                    result = Keyboard.current.yKey;
                    break;
                case "KEY_U":
                    result = Keyboard.current.uKey;
                    break;
                case "KEY_I":
                    result = Keyboard.current.iKey;
                    break;
                case "KEY_O":
                    result = Keyboard.current.oKey;
                    break;
                case "KEY_p":
                    result = Keyboard.current.pKey;
                    break;
                case "KEY_A":
                    result = Keyboard.current.aKey;
                    break;
                case "KEY_SPACE":
                    result = Keyboard.current.spaceKey;
                    break;
                case "KEY_LEFT_SHIFT":
                    result = Keyboard.current.leftShiftKey;
                    break;
                case "MOUSE_L":
                    result = Mouse.current.leftButton;
                    break;
                case "MOUSE_R":
                    result = Mouse.current.rightButton;
                    break;
            }
        }
        return result;
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
        aPress = this.getBinding(aBinding).wasPressedThisFrame;
        aHold = this.getBinding(aBinding).isPressed;
        // B
        bPress = this.getBinding(bBinding).wasPressedThisFrame;
        bHold = this.getBinding(bBinding).isPressed;
        // X
        xPress = this.getBinding(xBinding).wasPressedThisFrame;
        xHold = this.getBinding(xBinding).isPressed;
        // Y
        yPress = this.getBinding(yBinding).wasPressedThisFrame;
        yHold = this.getBinding(yBinding).isPressed;

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

        // DPAD
        dpadDownPress = Gamepad.all[joystickID].dpad.down.wasPressedThisFrame;
        dpadLeftPress = Gamepad.all[joystickID].dpad.left.wasPressedThisFrame;
        dpadRightPress = Gamepad.all[joystickID].dpad.right.wasPressedThisFrame;
        dpadUpPress = Gamepad.all[joystickID].dpad.up.wasPressedThisFrame;

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

        if (Keyboard.current.f5Key.wasPressedThisFrame) UnityEngine.SceneManagement.SceneManager.LoadScene("TesterMenu");
    }
}
