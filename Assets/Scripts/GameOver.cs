using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameOver : MonoBehaviour
{
    [Header("Game Over UI")]
    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text finalScoreText; // Ganti kembali ke Text (Font standar Unity)
    [SerializeField] private Font scoreFont; // Assign Sniglet-Regular.ttf di Inspector
    [SerializeField] private SecondChanceQuizManager secondChanceQuizManager;
    [SerializeField] private Transform playerRespawnPoint;
    [SerializeField] private float reviveInvulnerabilityTime = 1f;

    private bool isGameOver;
    private bool isWaitingForQuiz;
    private bool isReviving;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            SetupFinalScoreText();
        }

        if (secondChanceQuizManager == null)
        {
            secondChanceQuizManager = FindAnyObjectByType<SecondChanceQuizManager>();
        }

        if (secondChanceQuizManager == null)
        {
            GameObject quizManagerObject = new GameObject("Second Chance Quiz Manager");
            secondChanceQuizManager = quizManagerObject.AddComponent<SecondChanceQuizManager>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            HandleObstacleHit();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            HandleObstacleHit();
        }
    }

    public void HandleObstacleHit()
    {
        if (isGameOver || isWaitingForQuiz || isReviving)
            return;

        if (secondChanceQuizManager != null)
        {
            isWaitingForQuiz = true;

            secondChanceQuizManager.StartQuiz(isCorrect =>
            {
                isWaitingForQuiz = false;

                if (isCorrect)
                {
                    ContinueAfterSecondChance();
                }
                else
                {
                    ShowGameOver();
                }
            });

            return;
        }

        ShowGameOver();
    }

    private void ContinueAfterSecondChance()
    {
        if (playerRespawnPoint != null)
        {
            transform.position = playerRespawnPoint.position;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        StartCoroutine(ReviveCooldown());
    }

    private System.Collections.IEnumerator ReviveCooldown()
    {
        isReviving = true;
        yield return new WaitForSeconds(reviveInvulnerabilityTime);
        isReviving = false;
    }

    private void ShowGameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;

        if (gameOverPanel != null)
        {
            UpdateFinalScoreText();
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    private void SetupFinalScoreText()
    {
        if (finalScoreText != null || gameOverPanel == null)
            return;

        Transform existingFinalScore = gameOverPanel.transform.Find("Final Score Text");

        if (existingFinalScore != null)
        {
            finalScoreText = existingFinalScore.GetComponent<Text>();
            return;
        }

        // Buat GameObject baru dengan komponen Text (bawaan Unity)
        GameObject finalScoreObject = new GameObject("Final Score Text", typeof(Text));
        finalScoreObject.transform.SetParent(gameOverPanel.transform, false);

        RectTransform rectTransform = finalScoreObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(700f, 80f);

        finalScoreText = finalScoreObject.GetComponent<Text>();
        
        // Setup Font Sniglet (sama seperti quiz)
        finalScoreText.font = scoreFont != null ? scoreFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        finalScoreText.fontSize = 46; // Sesuaikan ukuran font
        finalScoreText.alignment = TextAnchor.MiddleCenter;
        
        // Warna Coklat Sesuai Gambar Pertama (#8B4513 - SaddleBrown)
        finalScoreText.color = new Color(0.545f, 0.271f, 0.075f);
        
        finalScoreText.horizontalOverflow = HorizontalWrapMode.Overflow;
        finalScoreText.verticalOverflow = VerticalWrapMode.Overflow;
    }

    private void UpdateFinalScoreText()
    {
        SetupFinalScoreText();

        if (finalScoreText == null)
            return;

        int finalScore = ScoreManager.Instance != null ? ScoreManager.Instance.CurrentScore : 0;
        finalScoreText.text = "Your Score = " + finalScore;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
