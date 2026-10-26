using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[InfoHeaderClass("Just drag me into the scene and forget about me =(")]
public class InputManager : MonoBehaviour
{   
    public static InputManager Instance { get; private set; }

    //[Header("Events (drag actions here in Inspector)")]
    [HideInInspector] public UnityEvent OnTap, OnPause;
    //private UnityEvent OnHold;
    [HideInInspector] public UnityEvent<float> OnSwipe; // passes swipe delta (-1 to 1)

    // Continuous input: -1 = left, 0 = neutral, 1 = right
    public float horizontalInput { get; private set; }

    private void Awake()
    {
        // Make sure there is only one InputManager.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // Keep the InputManager when changing scenes.
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        ReadHorizontalInput();
        CheckForLeftPress();
        CheckForRightPress();
        CheckForJumpPress();
        CheckForEscapePress();
    }  

    private void CheckForLeftPress()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = UnityEngine.InputSystem.Keyboard.current;

        if (keyboard != null && (keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame))
        {
            OnSwipe?.Invoke(-1f);
        }
#else
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnSwipe?.Invoke(-1f);
        }
#endif
    }

    private void CheckForRightPress()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = UnityEngine.InputSystem.Keyboard.current;

        if (keyboard != null && (keyboard.dKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame))
        {
            OnSwipe?.Invoke(1f);
        }
#else
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnSwipe?.Invoke(1f);
        }
#endif
    }

    private void ReadHorizontalInput()
    {
        horizontalInput = 0f;

#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = Keyboard.current;

        if(keyboard != null)
        {
            bool leftPressed = keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed;
            bool rightPressed = keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed;

            // Move left only when the left keys are pressed
            if(leftPressed && !rightPressed)
            {
                horizontalInput = -1f;
            }

            // Move right only when the right keys are pressed
            if (rightPressed && !leftPressed)
            {
                horizontalInput = 1f;
            }
        }
#else
        bool leftPressed = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);

        bool rightPressed = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

        if (leftPressed && !rightPressed)
        {
            HorizontalInput = -1f;
        }

        if (rightPressed && !leftPressed)
        {
            HorizontalInput = 1f;
        }
#endif
    }

    private void CheckForJumpPress()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = UnityEngine.InputSystem.Keyboard.current;

        if (keyboard != null && (keyboard.spaceKey.wasPressedThisFrame || keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame))
        {
            OnTap?.Invoke();
        }
#else
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W || Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnTap?.Invoke();
        }
#endif
    }

    private void CheckForEscapePress()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = UnityEngine.InputSystem.Keyboard.current;

        if (keyboard != null && (keyboard.escapeKey.wasPressedThisFrame))
        {
            OnPause.Invoke();
        }
#else
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnPause.Invoke();
        }
#endif
    }

}
