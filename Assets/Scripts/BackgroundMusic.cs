using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic backgroundMusic;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (backgroundMusic == null)
        {
            backgroundMusic = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    private void Update()
    {
        if (audioSource == null)
            return;

        if (Time.timeScale == 0f)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }
        else
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }
}
