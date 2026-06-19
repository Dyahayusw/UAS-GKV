using System;

[Serializable]
public class QuizQuestion
{
    public string question;
    public string[] answers = new string[4];
    public int correctAnswerIndex;
}
