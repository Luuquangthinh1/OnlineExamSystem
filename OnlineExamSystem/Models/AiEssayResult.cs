namespace OnlineExamSystem.Models
{
    public class AiEssayResult
    {
        public double Score
        {
            get; set;
        }

        public string Feedback
        {
            get; set;
        }
            = "";

        public double Accuracy
        {
            get; set;
        }

        public List<string>
            MatchedKeywords
        {
            get; set;
        }
            = new();

        public List<string>
            MissingKeywords
        {
            get; set;
        }
            = new();
    }
}