using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    // Variabel untuk menyimpan komponen AudioSource yang akan memainkan musik
    private AudioSource audioSource;

    private void Awake()
    {
        // Ambil komponen AudioSource dari objek ini
        audioSource = GetComponent<AudioSource>();

        // Jika tidak ada AudioSource, tambahkan otomatis
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Jika belum memainkan musik, langsung play
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    private void OnDisable()
    {
        // Ketika objek ini dinonaktifkan, hentikan musik
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    private void Update()
    {
        // Jika AudioSource null, hentikan update
        if (audioSource == null)
            return;

        // Jika game di-pause (timeScale = 0), pause musik
        if (Time.timeScale == 0f)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }
        // Jika game berjalan normal, resume musik jika sebelumnya di-pause
        else
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }
}
