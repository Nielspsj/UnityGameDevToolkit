using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DeathHandler : MonoBehaviour
{   
    [SerializeField, Header("Tag for objects that cause death")]
    private string obstacleTag = "Obstacle";
    [Header("Do you want to use checkpoints instead of losing?")]
    public bool useRespawnCheckpoints = true;
    [Header("Here you can add effects for when you hit the obstacles.")]
    public UnityEvent OnHittingObstacle;
    [Header("Give hit effects time to play before telling GameStateManager.")]
    public float eventDelay = 0.5f;


    //If it hits a collider with the obstacleTag then it will trigger the LoseGame event.
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(obstacleTag))
        {
            OnHittingObstacle?.Invoke();

            StartCoroutine(delayGameStateMessage());
        }
    }

    //If it hits a trigger with the obstacleTag then it will trigger the LoseGame event.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(obstacleTag))
        {
            OnHittingObstacle?.Invoke();

            StartCoroutine(delayGameStateMessage());            
        }
    }

    private IEnumerator delayGameStateMessage()
    {
        yield return new WaitForSeconds(eventDelay);

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
