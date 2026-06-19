using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
   public static ScoreManager Instance { get; private set; }

   [SerializeField] private Text scoreText;
   [SerializeField] private float scoreMultiplier = 1f;
   [SerializeField] private int minimumFontSize = 48;

   private float score;

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
      if (Time.timeScale == 0f || GameObject.FindGameObjectWithTag("Player") == null)
         return;

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

      if (scoreText.fontSize < minimumFontSize)
      {
         scoreText.fontSize = minimumFontSize;
      }

      scoreText.alignment = TextAnchor.MiddleCenter;
      scoreText.horizontalOverflow = HorizontalWrapMode.Overflow;
      scoreText.verticalOverflow = VerticalWrapMode.Overflow;
   }
}
