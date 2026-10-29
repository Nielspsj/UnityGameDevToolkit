using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField, Header("Drag into scene. Play music across levels or sound effects once.")]
    public AudioSource musicSource;
    public AudioSource soundEffectSource;

    private void Awake()
    {
        //Check if a SoundManager does not exist
        //Destroy this SoundManager if another exists
        //Don't destroy this SoundManager when we load a new scene
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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
