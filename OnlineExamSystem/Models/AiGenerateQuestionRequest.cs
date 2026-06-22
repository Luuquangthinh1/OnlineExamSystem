namespace OnlineExamSystem.Models
{
    public class AiGenerateQuestionRequest
    {
        public string Topic
        {
            get; set;
        } = "";

        public string Difficulty
        {
            get; set;
        } = "";

        public string QuestionType
        {
            get; set;
        } = "";

        public int NumberOfQuestions
        {
            get; set;
        }
    }
}