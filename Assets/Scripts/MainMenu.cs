using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Awake()
    {
        // Pastikan timeScale normal saat scene utama muncul
        Time.timeScale = 1f;
    }

    public void PlayGame()
    {
        // Set timeScale normal dan muat scene game (indeks 1)
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public void StopGame()
    {
        // Jika di Editor, hentikan play mode; jika di build, keluar aplikasi
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}
