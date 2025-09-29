using UnityEngine;
using static SwerveController;

public class ObstacleMover : MonoBehaviour
{    
    public enum MovementMode
    {
        Endless,    //Moves in one direction forever
        PingPong    //Moves back and forth forever
    }

    [SerializeField, Header("Switch between movement modes")]
    private MovementMode movementMode = MovementMode.Endless; // Switch mode in Inspector

    [Header("Does this obstacle move automatically? (e.g., down the track)")]
    [SerializeField] private Vector3 moveDirection = Vector3.zero;

    [Header("Speed of obstacle movement")]
    [SerializeField] private float speed = 0f;

    [SerializeField] private float patrolDistance = 3;
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        if (speed > 0)
        {
            transform.Translate(moveDirection * speed * Time.deltaTime);
        }
        if (movementMode == MovementMode.Endless)
        {
            return;
        }
        else if (movementMode == MovementMode.PingPong)
        {
            HandlePingPongMovement();
        }
    }


    private void HandlePingPongMovement()
    {
        if (Vector3.Distance(startPos, transform.position) > patrolDistance)
        {
            Debug.Log("past the distance, turn back!");
            moveDirection = -moveDirection;
        }
    }
}
