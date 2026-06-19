using System;
using UnityEngine;
using UnityEngine.UI;

public class SecondChanceQuizManager : MonoBehaviour
{
    [Header("Quiz UI")]
    [SerializeField] private GameObject quizPanel;
    [SerializeField] private Text questionText;
    [SerializeField] private Button[] answerButtons;

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

    private void Awake()
    {
        if (quizPanel == null || questionText == null || answerButtons == null || answerButtons.Length == 0)
        {
            CreateDefaultQuizUi();
        }

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
                answerText.text = currentQuestion.answers[i];
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
        panelImage.color = new Color(0f, 0f, 0f, 0.8f);

        questionText = CreateText("Question Text", quizPanel.transform, new Vector2(0.5f, 0.68f), new Vector2(1100f, 180f), 42, TextAnchor.MiddleCenter);
        answerButtons = new Button[4];

        for (int i = 0; i < answerButtons.Length; i++)
        {
            float y = 0.46f - (i * 0.12f);
            answerButtons[i] = CreateButton("Answer Button " + (i + 1), quizPanel.transform, new Vector2(0.5f, y), new Vector2(900f, 80f));
        }
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
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = Color.white;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;

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
        image.color = new Color(0.95f, 0.95f, 0.95f, 1f);

        Button button = buttonObject.GetComponent<Button>();
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(0.85f, 0.9f, 1f, 1f);
        colors.pressedColor = new Color(0.75f, 0.85f, 1f, 1f);
        button.colors = colors;

        Text buttonText = CreateText("Text", buttonObject.transform, new Vector2(0.5f, 0.5f), size, 30, TextAnchor.MiddleCenter);
        buttonText.color = Color.black;

        return button;
    }
}
