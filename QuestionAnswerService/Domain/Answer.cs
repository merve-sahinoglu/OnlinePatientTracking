namespace QuestionAnswerService.Domain
{
    public class Answer
    {
        public int Id { get; set; }
        public string AnswersText { get; set; }
        public int QuestionId { get; set; }
        public int AnsweredUserId { get; set; }
    }
}
