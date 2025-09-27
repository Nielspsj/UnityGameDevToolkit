using UnityEngine;

public class DeathHandler : MonoBehaviour
{
    [TextArea(1, 10)]
    [SerializeField]
    private string helpInfo = "Put me on the player";

    [SerializeField, Header("Tag for objects that cause death")]
    private string obstacleTag = "Obstacle";

    //If it hits a collider with the obstacleTag then it will trigger the LoseGame event.
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(obstacleTag))
        {
            Debug.Log("hit obstacle by tag: " + collision.transform.tag);

            GameStateManager.Instance.LoseGame();
        }
    }

    //If it hits a trigger with the obstacleTag then it will trigger the LoseGame event.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(obstacleTag))
        {
            GameStateManager.Instance.LoseGame();
        }
    }
}
