using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField, Header("Play music across levels")]
    public AudioSource musicSource;
    //public AudioSource sfxSource;

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


    //public void PlaySFX(AudioClip clip)
    //{
    //    sfxSource.PlayOneShot(clip);
    //}
}
