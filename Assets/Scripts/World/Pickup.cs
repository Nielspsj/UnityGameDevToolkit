using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField, Tooltip("How many points this pickup is worth")]
    private int value = 10;

    [SerializeField, Tooltip("Optional sound effect")]
    private AudioClip pickupSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ScoreManager.Instance.AddScore(value);

            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            Destroy(gameObject);
        }
    }
}
