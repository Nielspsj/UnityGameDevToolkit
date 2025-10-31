using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField, Header("Drag into scene. Play music across levels or sound effects once.")]
    public AudioSource musicSource;
    public AudioSource soundEffectSource;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayMusic();
    }

    public void PlayMusic()
    {        
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        soundEffectSource.PlayOneShot(clip);
    }
}
