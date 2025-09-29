using UnityEngine;
using UnityEngine.Events;

[InfoHeaderClass("Just drag me into the scene and forget about me =(")]
public class InputManager : MonoBehaviour
{
    /*
    [TextArea(1, 10)]
    [SerializeField]
    private string helpInfo = "Just drag me into the scene and forget about me =(";
    */
    public static InputManager Instance { get; private set; }

    //[Header("Events (drag actions here in Inspector)")]
    [HideInInspector] public UnityEvent OnTap;
    //private UnityEvent OnHold;
    [HideInInspector] public UnityEvent<float> OnSwipe; // passes swipe delta (-1 to 1)

    private Vector2 lastPosition;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            lastPosition = Input.mousePosition;
            OnTap?.Invoke();
        }

        /*
        if (Input.GetMouseButton(0))
        {
            //OnHold?.Invoke();
            Vector2 delta = (Vector2)Input.mousePosition - lastPosition;
            float normalizedDelta = Mathf.Clamp(delta.x / Screen.width, -1f, 1f);
            OnSwipe?.Invoke(normalizedDelta);
            lastPosition = Input.mousePosition;
        }
        */

        if (Input.GetKeyDown(KeyCode.A))
        {
            OnSwipe?.Invoke(-1f);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            OnSwipe?.Invoke(1f);
        }
    }
}
