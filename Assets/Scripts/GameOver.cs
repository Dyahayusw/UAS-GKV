using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameOver : MonoBehaviour
{
    // Header kategori untuk UI Game Over di Inspector
    [Header("Game Over UI")]
    [Header("Game Over UI")]
    // Panel UI yang muncul saat game over
    [SerializeField] private GameObject gameOverPanel;
    // Teks untuk menampilkan skor akhir
    [SerializeField] private Text finalScoreText;
    // Font yang digunakan untuk teks skor akhir
    [SerializeField] private Font scoreFont; 
    // Referensi ke manajer quiz untuk kesempatan kedua
    [SerializeField] private SecondChanceQuizManager secondChanceQuizManager;
    // Titik respawn player setelah kesempatan kedua
    [SerializeField] private Transform playerRespawnPoint;
    // Durasi player tidak bisa mati setelah respawn
    [SerializeField] private float reviveInvulnerabilityTime = 1f;

    // Status apakah game sudah over
    private bool isGameOver;
    // Status apakah sedang menunggu jawaban quiz
    private bool isWaitingForQuiz;
    // Status apakah player sedang revive
    private bool isReviving;
    // Referensi Rigidbody2D untuk kontrol fisik player
    private Rigidbody2D rb;

    private void Awake()
    {
        // Set timeScale normal agar game berjalan seperti biasa
        Time.timeScale = 1f;
        // Ambil komponen Rigidbody2D dari objek ini
        rb = GetComponent<Rigidbody2D>();

        // Jika panel game over tersedia, sembunyikan dulu dan setup teks skor
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            SetupFinalScoreText();
        }

        // Coba cari SecondChanceQuizManager di scene
        if (secondChanceQuizManager == null)
        {
            secondChanceQuizManager = FindAnyObjectByType<SecondChanceQuizManager>();
        }

        // Jika belum ada, buat baru secara otomatis
        if (secondChanceQuizManager == null)
        {
            GameObject quizManagerObject = new GameObject("Second Chance Quiz Manager");
            secondChanceQuizManager = quizManagerObject.AddComponent<SecondChanceQuizManager>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Jika player terkena trigger obstacle, tangani tabrakan
        if (other.CompareTag("Obstacle"))
        {
            HandleObstacleHit();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Jika player bertabrakan dengan obstacle (collision), tangani tabrakan
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            HandleObstacleHit();
        }
    }

    public void HandleObstacleHit()
    {
        // Jika sudah game over, sedang tunggu quiz, atau sedang revive, abaikan
        if (isGameOver || isWaitingForQuiz || isReviving)
            return;

        // Jika ada quiz manager, mulai quiz untuk kesempatan kedua
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

        // Jika tidak ada quiz, langsung game over
        ShowGameOver();
    }

    private void ContinueAfterSecondChance()
    {
        // Jika ada titik respawn, teleport player ke sana
        if (playerRespawnPoint != null)
        {
            transform.position = playerRespawnPoint.position;
        }

        // Hentikan gerakan player dan rotasi
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // Mulai cooldown invulnerbility setelah revive
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

        // Tampilkan panel game over dan perbarui teks skor
        if (gameOverPanel != null)
        {
            UpdateFinalScoreText();
            gameOverPanel.SetActive(true);
        }

        // Stop game dengan timeScale = 0
        Time.timeScale = 0f;
    }

    private void SetupFinalScoreText()
    {
        if (finalScoreText != null || gameOverPanel == null)
            return;

        // Coba cari objek "Final Score Text" di dalam panel game over
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

    public void BackHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
