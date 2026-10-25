// =====================================================================
//  SimpleAutoRun.cs
//  You do NOT need to add this yourself: EndlessRunnerGenerator adds it
//  to your player automatically. It runs forward along +Z, switches lanes
//  (A/D or Left/Right arrows) and jumps (Space / W / Up arrow).
//  Works with both the old and the new Unity Input System.
//  Don't want it? Untick "Add Auto Run To Player" on the generator.
// =====================================================================
using UnityEngine;

public class SimpleAutoRun : MonoBehaviour
{
    public float speed = 10f;
    public float speedIncreasePerSecond = 0.1f;
    public float maxSpeed = 25f;
    public float laneChangeSpeed = 15f;
    public float jumpHeight = 1.5f;
    public float gravity = 30f;

    EndlessRunnerGenerator gen;
    int lane;
    float x, groundY, yVelocity;
    bool grounded = true;

    void Start()
    {
        gen = FindFirstObjectByType<EndlessRunnerGenerator>();
        lane = gen.lanes / 2;
        x = gen.LaneX(lane);
        groundY = transform.position.y;
    }

    void Update()
    {
        float dt = Time.deltaTime;
        speed = Mathf.Min(maxSpeed, speed + speedIncreasePerSecond * dt);

        if (LeftPressed()) lane = Mathf.Max(0, lane - 1);
        if (RightPressed()) lane = Mathf.Min(gen.lanes - 1, lane + 1);
        if (JumpPressed() && grounded)
        {
            yVelocity = Mathf.Sqrt(2f * gravity * jumpHeight);
            grounded = false;
        }

        x = Mathf.MoveTowards(x, gen.LaneX(lane), laneChangeSpeed * dt);

        float y = transform.position.y;
        if (!grounded)
        {
            yVelocity -= gravity * dt;
            y += yVelocity * dt;
            if (y <= groundY) { y = groundY; yVelocity = 0f; grounded = true; }
        }

        transform.position = new Vector3(x, y, transform.position.z + speed * dt);
    }

    bool LeftPressed()
    {
#if ENABLE_INPUT_SYSTEM
        var k = UnityEngine.InputSystem.Keyboard.current;
        return k != null && (k.aKey.wasPressedThisFrame || k.leftArrowKey.wasPressedThisFrame);
#else
        return Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
#endif
    }

    bool RightPressed()
    {
#if ENABLE_INPUT_SYSTEM
        var k = UnityEngine.InputSystem.Keyboard.current;
        return k != null && (k.dKey.wasPressedThisFrame || k.rightArrowKey.wasPressedThisFrame);
#else
        return Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
#endif
    }

    bool JumpPressed()
    {
#if ENABLE_INPUT_SYSTEM
        var k = UnityEngine.InputSystem.Keyboard.current;
        return k != null && (k.spaceKey.wasPressedThisFrame || k.wKey.wasPressedThisFrame || k.upArrowKey.wasPressedThisFrame);
#else
        return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
#endif
    }
}
