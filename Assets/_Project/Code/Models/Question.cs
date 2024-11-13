public class Question
{
    [JsonProperty("tresc")]
    public string Content { get; private set; }

    [JsonProperty("poprawneOdpowiedzi")]
    private List<string> correctAnswers = new List<string>();

    [JsonProperty("podpowiedzi")]
    private List<string> answers;

    public Question(string content, List<string> correctAnswers, string answerA, string answerB, string answerC, string answerD)
    {
        this.Content = content;
        this.correctAnswers = correctAnswers;

        answers = new List<string> { answerA, answerB, answerC, answerD };
    }

    public bool IsCorrect(string answer)
    {
        return correctAnswers.Contains(answer.Trim().ToLower());
    }

    public string GetHint()
    {
        Random rand = new Random();
        int n = answers.Count;
        for (int i = 0; i < n - 1; i++)
        {
            int j = rand.Next(i, n);

            string temp = answers[i];
            answers[i] = answers[j];
            answers[j] = temp;
        }

        return $"A: {answers[0]}, B: {answers[1]}, C: {answers[2]}, D: {answers[3]}";
    }

    public string Serialize()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public static Question Deserialize(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("The specified file does not exist.");
        }

        string json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<Question>(json);
    }
}
