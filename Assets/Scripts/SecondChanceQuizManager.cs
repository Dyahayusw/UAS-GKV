using System;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Kelas utama pengelola kuis "kesempatan kedua", yang muncul saat pemain gagal dan diberi kesempatan menjawab pertanyaan agar tidak langsung Game Over.
public class SecondChanceQuizManager : MonoBehaviour
{
    // ------------ VARIABEL SERIALIZED UI KUIS ------------

    [Header("Quiz UI")]
    // Panel utama yang menampung seluruh elemen UI kuis.
    [SerializeField] private GameObject quizPanel;
    // Gambar latar belakang yang digunakan untuk panel kuis.
    [SerializeField] private Sprite quizBackgroundSprite;
    // Font teks yang digunakan untuk pertanyaan dan jawaban kuis.
    [SerializeField] private Font quizFont;
    // Komponen teks Unity UI untuk menampilkan pertanyaan.
    [SerializeField] private Text questionText;
    // Array tombol yang berfungsi sebagai pilihan jawaban (A, B, C, D).
    [SerializeField] private Button[] answerButtons;

#if UNITY_EDITOR
    // Bagian editor: pengaturan untuk memunculkan UI kuis di dalam Hierarchy editor.
    [Header("Editor Preview")]
    // Aktifkan agar UI quiz dibuat sebagai object scene, sehingga bisa diedit dari Hierarchy.
    [Tooltip("Aktifkan agar UI quiz dibuat sebagai object scene, sehingga bisa diedit dari Hierarchy.")]
    [SerializeField] private bool showQuizUiInEditor = true;
#endif

    // Bagian pengaturan offset teks jawaban agar pas di area merah sprite banner.
    [Header("Answer Text Offset (sesuaikan agar pas di tengah area merah)")]
    // Geser teks jawaban secara vertikal (pixel). Nilai negatif = turun, positif = na-Re-.
    [Tooltip("Geser teks jawaban secara vertikal (pixel). Nilai negatif = turun, positif = naik.")]
    [SerializeField] private float answerTextVerticalOffset = 10f;
    // Lebar area teks relatif terhadap tombol (0-1). Kecilkan jika teks terlalu lebar untuk area merah.
    [Tooltip("Lebar area teks relatif terhadap tombol (0-1). Kecilkan jika teks terlalu lebar untuk area merah.")]
    [SerializeField] private float answerTextWidthRatio = 0.75f;
    // Tinggi area teks relatif terhadap tombol (0-1). Kecilkan agar teks tidak menabrak daun di atas/bawah.
    [Tooltip("Tinggi area teks relatif terhadap tombol (0-1). Kecilkan agar teks tidak menabrak daun di atas/bawah.")]
    [SerializeField] private float answerTextHeightRatio = 0.45f;

    // Bagian data pertanyaan kuis.
    [Header("Questions")]
    // Array berisi semua pertanyaan dan jawaban kuis yang tersedia.
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

    // ------------ VARIABEL PRIVATE INTERNAL ------------

    // Callback yang dipanggil ketika kuis selesai, parameter bool menunjukkan apakah jawaban benar.
    private Action<bool> onQuizFinished;
    // Menyimpan referensi pertanyaan yang sedang aktif dalam kuis.
    private QuizQuestion currentQuestion;
    // Label prefix untuk setiap pilihan jawaban (A, B, C, D).
    private readonly string[] answerLabels = { "A. ", "B. ", "C. ", "D. " };

#if UNITY_EDITOR
    // Flag internal untuk mencegah refresh editor berulang kali dalam satu frame.
    private bool editorRefreshQueued;

    // Dipanggil otomatis oleh Unity Editor ketika ada perubahan nilai di Inspector.
    private void OnValidate()
    {
        // Muat ulang sprite latar belakang default jika belum diatur.
        if (quizBackgroundSprite == null)
        {
            quizBackgroundSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Assets/Sprites/assets (7).png");
        }

        // Muat ulang font kuis default jika belum diatur.
        if (quizFont == null)
        {
            quizFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/Sniglet-Regular.ttf");
        }

        // Lakukan update posisi teks jawaban di Editor saat slider offset diubah dan sedang tidak bermain.
        if (!Application.isPlaying && showQuizUiInEditor)
        {
            QueueEditorQuizUiRefresh();
        }

        RepositionAnswerTexts();
    }

    // Menjadwalkan pembaruan UI kuis di editor agar tidak terjadi terlalu sering.
    private void QueueEditorQuizUiRefresh()
    {
        if (editorRefreshQueued)
            return;

        editorRefreshQueued = true;
        EditorApplication.delayCall += () =>
        {
            editorRefreshQueued = false;

            if (this == null || Application.isPlaying || !showQuizUiInEditor)
                return;

            EnsureQuizUiExistsInScene();
            SetQuizUiVisibleInEditor(true);
        };
    }

    // Membuat atau memperbarui UI kuis di dalam scene editor agar dapat diedit dari Hierarchy.
    [ContextMenu("Create/Refresh Quiz UI In Scene")]
    private void EnsureQuizUiExistsInScene()
    {
        LoadDefaultBackgroundSprite();
        LoadDefaultQuizFont();

        if (quizPanel == null || questionText == null || answerButtons == null || answerButtons.Length == 0)
        {
            CreateDefaultQuizUi();
        }

        ApplyQuizTextStyle();
        RepositionAnswerTexts();

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorUtility.SetDirty(this);
            if (quizPanel != null)
            {
                EditorUtility.SetDirty(quizPanel);
            }
        }
#endif
    }

    // Mengatur visibilitas panel kuis di editor (hanya untuk keperluan preview).
    private void SetQuizUiVisibleInEditor(bool isVisible)
    {
        if (quizPanel != null)
        {
            quizPanel.SetActive(isVisible);
        }
    }
#endif

    // Dipanggil sekali saat object ini pertama kali diinisialisasi, sebelum Start.
    private void Awake()
    {
        LoadDefaultBackgroundSprite();
        LoadDefaultQuizFont();

        // Jika referensi UI belum diisi, buat UI kuis default secara runtime.
        if (quizPanel == null || questionText == null || answerButtons == null || answerButtons.Length == 0)
        {
            CreateDefaultQuizUi();
        }

        ApplyQuizTextStyle();
        RepositionAnswerTexts();
        // Sembunyikan panel kuis sampai dipanggil nanti.
        quizPanel.SetActive(false);
    }

    // Memulai proses kuis, dipilih pertanyaan secara acak dan UI槽UI ditampilkan.
    public void StartQuiz(Action<bool> callback)
    {
        // Pastikan ada pertanyaan yang tersedia sebelum memulai kuis.
        if (questions == null || questions.Length == 0)
        {
            Debug.LogWarning("Pertanyaan quiz belum diisi. Player langsung Game Over.");
            callback?.Invoke(false);
            return;
        }

        onQuizFinished = callback;
        // Pilih pertanyaan secara acak dari daftar.
        currentQuestion = questions[UnityEngine.Random.Range(0, questions.Length)];

        quizPanel.SetActive(true);
        Time.timeScale = 0f; // Hentikan waktu game saat kuis berlangsung.

        questionText.text = currentQuestion.question;

        // Iterasi semua tombol jawaban dan atur teks serta listener sesuai data pertanyaan.
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

    // Menyelesaikan kuis, sembunyikan UI dan kembalikan waktu game ke normal.
    private void FinishQuiz(bool isCorrect)
    {
        quizPanel.SetActive(false);
        Time.timeScale = 1f; // Kembalikan waktu game ke normal.
        onQuizFinished?.Invoke(isCorrect);
        // Hapus referensi callback agar tidak ada leak memory.
        onQuizFinished = null;
    }

    // Membuat UI kuis default jika belum ada di scene ( Canvas, panel, teks, tombol ).
    private void CreateDefaultQuizUi()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();

        if (canvas == null)
        {
            // Buat Canvas baru jika belum ada di scene.
            GameObject canvasObject = new GameObject("Quiz Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObject.GetComponent<Canvas>();
            canvas .renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler canvasScaler = canvasObject.GetComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
        }

        // Cek apakah panel kuis sudah ada, jika ya gunakan yang sudah ada.
        Transform existingQuizPanel = canvas.transform.Find("Second Chance Quiz Panel");
        if (existingQuizPanel != null)
        {
            quizPanel = existingQuizPanel.gameObject;
        }
        else
        {
            quizPanel = new GameObject("Second Chance Quiz Panel", typeof(Image));
            quizPanel.transform.SetParent(canvas.transform, false);
        }

        if (quizPanel.GetComponent<Image>() == null)
        {
            quizPanel.AddComponent<Image>();
        }

        RectTransform panelRect = quizPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // Atur properti visual panel utama kuis.
        Image panelImage = quizPanel.GetComponent<Image>();
        panelImage.sprite = quizBackgroundSprite;
        panelImage.color = Color.white;
        panelImage.preserveAspect = false;

        // Inisialisasi teks pertanyaan dan tombol jawaban.
        questionText = FindOrCreateText("Question Text", quizPanel.transform, new Vector2(0.5f, 0.675f), new Vector2(1050f, 170f), 46, TextAnchor.MiddleCenter);
        answerButtons = new Button[4];

        answerButtons[0] = FindOrCreateButton("Answer Button A", quizPanel.transform, new Vector2(0.34f, 0.43f), new Vector2(360f, 95f));
        answerButtons[1] = FindOrCreateButton("Answer Button B", quizPanel.transform, new Vector2(0.34f, 0.265f), new Vector2(360f, 95f));
        answerButtons[2] = FindOrCreateButton("Answer Button C", quizPanel.transform, new Vector2(0.66f, 0.43f), new Vector2(360f, 95f));
        answerButtons[3] = FindOrCreateButton("Answer Button D", quizPanel.transform, new Vector2(0.66f, 0.265f), new Vector2(360f, 95f));
    }

    // Mencari atau membuat komponen teks UI dengan konfigurasi tertentu.
    private Text FindOrCreateText(string objectName, Transform parent, Vector2 anchor, Vector2 size, int fontSize, TextAnchor alignment)
    {
        Transform existingText = parent.Find(objectName);
        if (existingText != null && existingText.TryGetComponent(out Text text))
        {
            ConfigureRectTransform(text.GetComponent<RectTransform>(), anchor, size);
            ApplyTextStyle(text, fontSize);
            return text;
        }

        return CreateText(objectName, parent, anchor, size, fontSize, alignment);
    }

    // Membuat komponen teks UI baru dengan properti default yang sesuai untuk kuis.
    private Text CreateText(string objectName, Transform parent, Vector2 anchor, Vector2 size, int fontSize, TextAnchor alignment)
    {
        GameObject textObject = new GameObject(objectName, typeof(Text));
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        ConfigureRectTransform(rectTransform, anchor, size);

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

        // Tambahkan efek bayangan pada teks agar lebih terbaca.
        Shadow shadow = textObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0.28f, 0.12f, 0.02f, 0.35f);
        shadow.effectDistance = new Vector2(2f, -2f);

        return text;
    }

    // Mencari atau membuat tombol jawaban dengan teks di dalamnya.
    private Button FindOrCreateButton(string objectName, Transform parent, Vector2 anchor, Vector2 size)
    {
        Transform existingButton = parent.Find(objectName);
        if (existingButton != null && existingButton.TryGetComponent(out Button button))
        {
            ConfigureRectTransform(button.GetComponent<RectTransform>(), anchor, size);

            Text buttonText = button.GetComponentInChildren<Text>();
            if (buttonText == null)
            {
                buttonText = CreateText("Text", button.transform, new Vector2(0.5f, 0.5f), new Vector2(size.x - 24f, size.y), 30, TextAnchor.MiddleCenter);
            }

            ApplyTextStyle(buttonText, 30);
            return button;
        }

        return CreateButton(objectName, parent, anchor, size);
    }

    // Membuat tombol jawaban baru dengan styling standar (transparan saat normal).
    private Button CreateButton(string objectName, Transform parent, Vector2 anchor, Vector2 size)
    {
        GameObject buttonObject = new GameObject(objectName, typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);

        RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
        ConfigureRectTransform(rectTransform, anchor, size);

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

    // Mengatur properti RectTransform seperti anchor dan ukuran.
    private void ConfigureRectTransform(RectTransform rectTransform, Vector2 anchor, Vector2 size)
    {
        rectTransform.anchorMin = anchor;
        rectTransform.anchorMax = anchor;
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = Vector2.zero;
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

            // Hitung ukuran area teks berdasarkan ratio agar pas di area merah sprite.
            float width = buttonRect.sizeDelta.x * answerTextWidthRatio;
            float height = buttonRect.sizeDelta.y * answerTextHeightRatio;
            textRect.sizeDelta = new Vector2(width, height);
            textRect.anchoredPosition = new Vector2(0f, answerTextVerticalOffset);
        }
    }

    // Memuat gambar latar belakang default jika belum ada referensi.
    private void LoadDefaultBackgroundSprite()
    {
        if (quizBackgroundSprite != null)
            return;

#if UNITY_EDITOR
        quizBackgroundSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Assets/Sprites/assets (7).png");
#endif
    }

    // Memuat font kuis default jika belum ada referensi.
    private void LoadDefaultQuizFont()
    {
        if (quizFont != null)
            return;

#if UNITY_EDITOR
        quizFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/Sniglet-Regular.ttf");
#endif
    }

    // Menerapkan gaya teks (font, ukuran, dll.) pada pertanyaan dan semua teks jawaban.
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

    // Menerapkan gaya standar pada komponen teks UI (font, warna, wrapping, dsb.).
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

        // Tambahkan efek bayangan jika belum ada.
        if (text.GetComponent<Shadow>() == null)
        {
            Shadow shadow = text.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0.28f, 0.12f, 0.02f, 0.35f);
            shadow.effectDistance = new Vector2(2f, -2f);
        }
    }
}