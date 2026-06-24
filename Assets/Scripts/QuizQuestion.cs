using System;

// Serializable memungkinkan kelas ini ditampilkan dan di-edit di Inspector Unity
[Serializable]
public class QuizQuestion
{
    // Teks pertanyaan quiz
    public string question;
    // Array untuk menyimpan 4 opsi jawaban
    public string[] answers = new string[4];
    // Indeks jawaban yang benar di dalam array answers
    public int correctAnswerIndex;
}
