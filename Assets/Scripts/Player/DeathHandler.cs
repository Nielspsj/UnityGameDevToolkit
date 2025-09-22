using UnityEngine;

public class DeathHandler : MonoBehaviour
{
    [SerializeField, Tooltip("Tag for objects that cause death")]
    private string obstacleTag = "Obstacle";

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(obstacleTag))
        {
            Debug.Log("hit obstacle by tag: " + collision.transform.tag);

            GameStateManager.Instance.LoseGame();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(obstacleTag))
        {
            GameStateManager.Instance.LoseGame();
        }
    }
}
