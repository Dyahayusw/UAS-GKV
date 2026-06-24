using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
   // Singleton pattern: akses ScoreManager dari script lain via ScoreManager.Instance
   public static ScoreManager Instance { get; private set; }

   // Referensi UI Text untuk menampilkan skor
   [SerializeField] private Text scoreText;
   // Pengali skor per detik
   [SerializeField] private float scoreMultiplier = 1f;
   // Ukuran font minimum agar teks tetap terbaca
   [SerializeField] private int minimumFontSize = 48;

   // Variabel internal untuk menyimpan skor (terbatas float, tetapi ditampilkan sebagai int)
   private float score;

   // Property untuk mengakses skor saat ini sebagai integer
   public int CurrentScore => (int)score;

   private void Awake()
   {
      Instance = this;
   }

   private void Start()
   {
      SetupScoreText();
      UpdateScoreText();
   }

   private void Update()
   {
      // Jika game dipause atau player sudah tidak ada, jangan tambah skor
      if (Time.timeScale == 0f || GameObject.FindGameObjectWithTag("Player") == null)
         return;

      // Tambah skor setiap frame berdasarkan scoreMultiplier dan waktu delta
      score += scoreMultiplier * Time.deltaTime;
      UpdateScoreText();
   }

   private void UpdateScoreText()
   {
      if (scoreText != null)
      {
         scoreText.text = ((int)score).ToString();
      }
   }

   private void SetupScoreText()
   {
      if (scoreText == null)
      {
         scoreText = GetComponent<Text>();
      }

      if (scoreText == null)
         return;

      // Pastikan ukuran font tidak lebih kecil than minimumFontSize
      if (scoreText.fontSize < minimumFontSize)
      {
         scoreText.fontSize = minimumFontSize;
      }

      // Atur alignment dan overflow
      scoreText.alignment = TextAnchor.MiddleCenter;
      scoreText.horizontalOverflow = HorizontalWrapMode.Overflow;
      scoreText.verticalOverflow = VerticalWrapMode.Overflow;
   }
}
