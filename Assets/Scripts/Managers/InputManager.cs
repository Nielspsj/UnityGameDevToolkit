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

    //private Vector2 lastPosition;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        LeftPressed();
        RightPressed();
        JumpPressed();
        EscapePressed();
    }  

    private void LeftPressed()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard k = UnityEngine.InputSystem.Keyboard.current;

        if (k != null && (k.aKey.wasPressedThisFrame || k.leftArrowKey.wasPressedThisFrame))
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

    private void RightPressed()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard k = UnityEngine.InputSystem.Keyboard.current;

        if (k != null && (k.dKey.wasPressedThisFrame || k.rightArrowKey.wasPressedThisFrame))
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

    private void JumpPressed()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard k = UnityEngine.InputSystem.Keyboard.current;

        if (k != null && (k.spaceKey.wasPressedThisFrame || k.wKey.wasPressedThisFrame || k.upArrowKey.wasPressedThisFrame))
        {
            Debug.Log("jumpPressed");
            OnTap?.Invoke();
        }
#else
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W || Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnTap?.Invoke();
        }
#endif
    }

    private void EscapePressed()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard k = UnityEngine.InputSystem.Keyboard.current;

        if (k != null && (k.escapeKey.wasPressedThisFrame))
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
