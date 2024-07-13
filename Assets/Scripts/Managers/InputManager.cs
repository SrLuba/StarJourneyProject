using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public LEInput.InputManager engine;

    public List<BindingSO> bindings;

    public bool swapLR = false;


    public Vector2 leftStick = new Vector2(0f, 0f);
    public Vector2 rightStick = new Vector2(0f, 0f);

    public AudioClip JoystickClip, KeyboardClip, DefaultErrorClip;

    public Animator animInputShower;



    public void Awake()
    {
        if (instance != null) {
            Destroy(instance.gameObject);
        }
        instance = this;
   
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        engine = new LEInput.InputManager();
        engine.Start(); // we start the input engine

        engine.bindings = bindings[0].bindings;
        engine.SetupAnimator(animInputShower); // we setup the animator that shows up when you press something
        engine.EnableLookUp(); // Changing from keyboard to gamepad in realtime
        engine.HookMouse(); // Lock Mouse
        engine.UpdateBindings(false); // we update bindings

    }

    // Update is called once per frame
    void Update()
    {
        if (bindings.Count <= 0)
        {
            bindings.Add(Resources.Load<BindingSO>("Data/DefaultBindings"));
            engine.bindings = bindings[0].bindings;
        }
        engine.Update(); // Engine Update

        leftStick = engine.getAxis(engine.getInputBinding("MOVE"));
        rightStick = engine.getAxis(engine.getInputBinding("CAMERA"));

        if (Keyboard.current.f5Key.wasPressedThisFrame) UnityEngine.SceneManagement.SceneManager.LoadScene("TesterMenu");
    }
}
