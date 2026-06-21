using System;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SecondChanceQuizManager : MonoBehaviour
{
    [Header("Quiz UI")]
    [SerializeField] private GameObject quizPanel;
    [SerializeField] private Sprite quizBackgroundSprite;
    [SerializeField] private Font quizFont;
    [SerializeField] private Text questionText;
    [SerializeField] private Button[] answerButtons;

    [Header("Answer Text Offset (sesuaikan agar pas di tengah area merah)")]
    [Tooltip("Geser teks jawaban secara vertikal (pixel). Nilai negatif = turun, positif = naik.")]
    [SerializeField] private float answerTextVerticalOffset = 10f;
    [Tooltip("Lebar area teks relatif terhadap tombol (0-1). Kecilkan jika teks terlalu lebar untuk area merah.")]
    [SerializeField] private float answerTextWidthRatio = 0.75f;
    [Tooltip("Tinggi area teks relatif terhadap tombol (0-1). Kecilkan agar teks tidak menabrak daun di atas/bawah.")]
    [SerializeField] private float answerTextHeightRatio = 0.45f;

    [Header("Questions")]
    [SerializeField] private QuizQuestion[] questions =
    {
        new QuizQuestion
        {
            question = "Kerajaan maritim besar yang berpusat di Sumatera adalah?",
            answers = new[] { "Majapahit", "Sriwijaya", "Singasari", "Kediri" },
            correctAnswerIndex = 1
        },
        new QuizQuestion
        {
            question = "Kerajaan Hindu tertua di Indonesia adalah?",
            answers = new[] { "Kutai", "Demak", "Banten", "Mataram Islam" },
            correctAnswerIndex = 0
        },
        new QuizQuestion
        {
            question = "Raja terkenal dari Kerajaan Majapahit adalah?",
            answers = new[] { "Hayam Wuruk", "Sultan Agung", "Purnawarman", "Balaputradewa" },
            correctAnswerIndex = 0
        },
        new QuizQuestion
        {
            question = "Kerajaan Islam pertama di Indonesia adalah?",
            answers = new[] { "Samudra Pasai", "Tarumanegara", "Pajajaran", "Singasari" },
            correctAnswerIndex = 0
        },
        new QuizQuestion
        {
            question = "Kerajaan Tarumanegara terkenal dengan raja bernama?",
            answers = new[] { "Purnawarman", "Hayam Wuruk", "Ken Arok", "Sultan Agung" },
            correctAnswerIndex = 0
        },
        new QuizQuestion
        {
            question = "Kerajaan Singasari didirikan oleh?",
            answers = new[] { "Ken Arok", "Gajah Mada", "Balaputradewa", "Mulawarman" },
            correctAnswerIndex = 0
        },
        new QuizQuestion
        {
            question = "Sumpah Palapa berkaitan dengan tokoh dari Majapahit bernama?",
            answers = new[] { "Gajah Mada", "Sultan Hasanuddin", "Purnawarman", "Airlangga" },
            correctAnswerIndex = 0
        },
        new QuizQuestion
        {
            question = "Kerajaan bercorak Buddha yang menjadi pusat pembelajaran di Sumatera adalah?",
            answers = new[] { "Sriwijaya", "Kutai", "Demak", "Banten" },
            correctAnswerIndex = 0
        },
        new QuizQuestion
        {
            question = "Kerajaan Islam di Jawa yang terkenal dengan Wali Songo adalah?",
            answers = new[] { "Demak", "Kutai", "Tarumanegara", "Kediri" },
            correctAnswerIndex = 0
        },
        new QuizQuestion
        {
            question = "Kerajaan Gowa-Tallo berada di wilayah?",
            answers = new[] { "Sulawesi Selatan", "Sumatera Barat", "Jawa Tengah", "Kalimantan Timur" },
            correctAnswerIndex = 0
        }
    };

    private Action<bool> onQuizFinished;
    private QuizQuestion currentQuestion;
    private readonly string[] answerLabels = { "A. ", "B. ", "C. ", "D. " };

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (quizBackgroundSprite == null)
        {
            quizBackgroundSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Assets/Sprites/assets (7).png");
        }

        if (quizFont == null)
        {
            quizFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/Sniglet-Regular.ttf");
        }

        // Live-update posisi teks jawaban di Editor saat slider offset diubah
        RepositionAnswerTexts();
    }
#endif

    private void Awake()
    {
        LoadDefaultBackgroundSprite();
        LoadDefaultQuizFont();

        if (quizPanel == null || questionText == null || answerButtons == null || answerButtons.Length == 0)
        {
            CreateDefaultQuizUi();
        }

        ApplyQuizTextStyle();
        RepositionAnswerTexts();
        quizPanel.SetActive(false);
    }

    public void StartQuiz(Action<bool> callback)
    {
        if (questions == null || questions.Length == 0)
        {
            Debug.LogWarning("Pertanyaan quiz belum diisi. Player langsung Game Over.");
            callback?.Invoke(false);
            return;
        }

        onQuizFinished = callback;
        currentQuestion = questions[UnityEngine.Random.Range(0, questions.Length)];

        quizPanel.SetActive(true);
        Time.timeScale = 0f;

        questionText.text = currentQuestion.question;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int answerIndex = i;
            Text answerText = answerButtons[i].GetComponentInChildren<Text>();

            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].gameObject.SetActive(currentQuestion.answers != null && i < currentQuestion.answers.Length);

            if (answerText != null && currentQuestion.answers != null && i < currentQuestion.answers.Length)
            {
                string label = i < answerLabels.Length ? answerLabels[i] : string.Empty;
                answerText.text = label + currentQuestion.answers[i];
            }

            answerButtons[i].onClick.AddListener(() => FinishQuiz(answerIndex == currentQuestion.correctAnswerIndex));
        }
    }

    private void FinishQuiz(bool isCorrect)
    {
        quizPanel.SetActive(false);
        Time.timeScale = 1f;
        onQuizFinished?.Invoke(isCorrect);
        onQuizFinished = null;
    }

    private void CreateDefaultQuizUi()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();

        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("Quiz Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler canvasScaler = canvasObject.GetComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
        }

        quizPanel = new GameObject("Second Chance Quiz Panel", typeof(Image));
        quizPanel.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = quizPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = quizPanel.GetComponent<Image>();
        panelImage.sprite = quizBackgroundSprite;
        panelImage.color = Color.white;
        panelImage.preserveAspect = false;

        questionText = CreateText("Question Text", quizPanel.transform, new Vector2(0.5f, 0.675f), new Vector2(1050f, 170f), 46, TextAnchor.MiddleCenter);
        answerButtons = new Button[4];

        answerButtons[0] = CreateButton("Answer Button A", quizPanel.transform, new Vector2(0.34f, 0.43f), new Vector2(360f, 95f));
        answerButtons[1] = CreateButton("Answer Button B", quizPanel.transform, new Vector2(0.34f, 0.265f), new Vector2(360f, 95f));
        answerButtons[2] = CreateButton("Answer Button C", quizPanel.transform, new Vector2(0.66f, 0.43f), new Vector2(360f, 95f));
        answerButtons[3] = CreateButton("Answer Button D", quizPanel.transform, new Vector2(0.66f, 0.265f), new Vector2(360f, 95f));
    }

    private Text CreateText(string objectName, Transform parent, Vector2 anchor, Vector2 size, int fontSize, TextAnchor alignment)
    {
        GameObject textObject = new GameObject(objectName, typeof(Text));
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchor;
        rectTransform.anchorMax = anchor;
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = Vector2.zero;

        Text text = textObject.GetComponent<Text>();
        text.font = quizFont != null ? quizFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = Color.white;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 24;
        text.resizeTextMaxSize = fontSize;

        Shadow shadow = textObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0.28f, 0.12f, 0.02f, 0.35f);
        shadow.effectDistance = new Vector2(2f, -2f);

        return text;
    }

    private Button CreateButton(string objectName, Transform parent, Vector2 anchor, Vector2 size)
    {
        GameObject buttonObject = new GameObject(objectName, typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);

        RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchor;
        rectTransform.anchorMax = anchor;
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = Vector2.zero;

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0f);

        Button button = buttonObject.GetComponent<Button>();
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(1f, 1f, 1f, 0.12f);
        colors.pressedColor = new Color(1f, 1f, 1f, 0.24f);
        button.colors = colors;

        // Teks dipusatkan tepat di tengah tombol (0.5, 0.5). Posisi & ukuran area
        // teks akan disesuaikan lagi oleh RepositionAnswerTexts() agar masuk ke
        // bagian merah dari sprite banner (menghindari daun hijau di atas/bawah).
        Text buttonText = CreateText("Text", buttonObject.transform, new Vector2(0.5f, 0.5f), new Vector2(size.x - 24f, size.y), 30, TextAnchor.MiddleCenter);
        buttonText.color = Color.white;

        return button;
    }

    /// <summary>
    /// Menyesuaikan posisi & ukuran teks jawaban supaya pas berada di area
    /// merah pada sprite banner, bukan menabrak daun hijau di atas/bawahnya.
    /// Atur nilai answerTextVerticalOffset / WidthRatio / HeightRatio di
    /// Inspector untuk fine-tuning sesuai sprite yang dipakai.
    /// </summary>
    private void RepositionAnswerTexts()
    {
        if (answerButtons == null)
            return;

        foreach (Button answerButton in answerButtons)
        {
            if (answerButton == null)
                continue;

            RectTransform buttonRect = answerButton.GetComponent<RectTransform>();
            Text answerText = answerButton.GetComponentInChildren<Text>();
            if (answerText == null || buttonRect == null)
                continue;

            RectTransform textRect = answerText.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.5f, 0.5f);
            textRect.anchorMax = new Vector2(0.5f, 0.5f);

            float width = buttonRect.sizeDelta.x * answerTextWidthRatio;
            float height = buttonRect.sizeDelta.y * answerTextHeightRatio;
            textRect.sizeDelta = new Vector2(width, height);
            textRect.anchoredPosition = new Vector2(0f, answerTextVerticalOffset);
        }
    }

    private void LoadDefaultBackgroundSprite()
    {
        if (quizBackgroundSprite != null)
            return;

#if UNITY_EDITOR
        quizBackgroundSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Assets/Sprites/assets (7).png");
#endif
    }

    private void LoadDefaultQuizFont()
    {
        if (quizFont != null)
            return;

#if UNITY_EDITOR
        quizFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/Sniglet-Regular.ttf");
#endif
    }

    private void ApplyQuizTextStyle()
    {
        ApplyTextStyle(questionText, 46);

        if (answerButtons == null)
            return;

        foreach (Button answerButton in answerButtons)
        {
            if (answerButton == null)
                continue;

            Text answerText = answerButton.GetComponentInChildren<Text>();
            ApplyTextStyle(answerText, 30);
        }
    }

    private void ApplyTextStyle(Text text, int maxFontSize)
    {
        if (text == null)
            return;

        if (quizFont != null)
        {
            text.font = quizFont;
        }

        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 22;
        text.resizeTextMaxSize = maxFontSize;

        if (text.GetComponent<Shadow>() == null)
        {
            Shadow shadow = text.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0.28f, 0.12f, 0.02f, 0.35f);
            shadow.effectDistance = new Vector2(2f, -2f);
        }
    }
}