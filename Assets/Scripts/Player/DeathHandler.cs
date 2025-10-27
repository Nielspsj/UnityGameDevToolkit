using UnityEngine;

public class DeathHandler : MonoBehaviour
{   
    [SerializeField, Header("Tag for objects that cause death")]
    private string obstacleTag = "Obstacle";
    public bool useRespawnCheckpoints = true;
    //If it hits a collider with the obstacleTag then it will trigger the LoseGame event.
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(obstacleTag))
        {
            //Debug.Log("hit obstacle by tag: " + collision.transform.tag);
            if (useRespawnCheckpoints == true)
            {
                GameStateManager.Instance.RespawnAtCheckpoint();
            }
            else
            {
                GameStateManager.Instance.LoseGame();
            }
        }
    }

    //If it hits a trigger with the obstacleTag then it will trigger the LoseGame event.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(obstacleTag))
        {
            if (useRespawnCheckpoints == true)
            {
                GameStateManager.Instance.RespawnAtCheckpoint();
            }
            else
            {
                GameStateManager.Instance.LoseGame();
            }
        }
    }
}
