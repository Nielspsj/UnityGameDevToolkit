using UnityEngine;

public class ObstacleMover : MonoBehaviour
{    
    [Header("Does this obstacle move automatically? (e.g., down the track)")]
    [SerializeField] private Vector3 moveDirection = Vector3.zero;

    [Header("Speed of obstacle movement")]
    [SerializeField] private float speed = 0f;

    private void Update()
    {
        if (speed > 0)
        {
            transform.Translate(moveDirection * speed * Time.deltaTime);
        }
    }
}
