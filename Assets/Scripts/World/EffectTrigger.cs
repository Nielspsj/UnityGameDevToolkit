using UnityEngine;
using UnityEngine.Events;

public class EffectTrigger : MonoBehaviour
{
    [Header("Effect Components (Optional)")]
    public ParticleSystem particleEffect;
    public Animator animator;
    public AudioSource audioSource;
    public string animationTriggerName;
    public GameObject objectToActivate;

    [Header("Settings")]
    public bool deactivateAfterPlay = false;
    public float deactivateDelay = 1f;

    public void PlayEffect()
    {
        if (particleEffect != null)
            particleEffect.Play();

        if (animator != null && !string.IsNullOrEmpty(animationTriggerName))
            animator.SetTrigger(animationTriggerName);

        if (audioSource != null)
            audioSource.Play();

        if (objectToActivate != null)
            objectToActivate.SetActive(true);

        if (deactivateAfterPlay && objectToActivate != null)
            StartCoroutine(DeactivateAfterDelay());
    }

    private System.Collections.IEnumerator DeactivateAfterDelay()
    {
        yield return new WaitForSeconds(deactivateDelay);
        objectToActivate.SetActive(false);
    }
}
