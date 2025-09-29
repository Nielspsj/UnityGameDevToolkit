using UnityEngine;

[InfoHeaderClass("Drag me into the level to be picked up by the player :D")]
public class Pickup : MonoBehaviour
{
    /*
    [TextArea(1, 10)]
    [SerializeField]
    private string helpInfo = "Drag me into the level to be picked up by the player :D";
    */
    [SerializeField, Header("How many points this pickup is worth")]
    private int value = 10;

    [SerializeField, Header("Optional sound effect")]
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
