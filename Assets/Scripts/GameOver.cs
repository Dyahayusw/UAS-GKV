using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;

    private bool isGameOver;

    private void Awake()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isGameOver && other.CompareTag("Obstacle"))
        {
            ShowGameOver();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isGameOver && collision.gameObject.CompareTag("Obstacle"))
        {
            ShowGameOver();
        }
    }

    private void ShowGameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
