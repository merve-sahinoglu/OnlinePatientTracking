namespace QuestionAnswerService.Domain
{
    public class Question
    {
        public Question(string questionText, int patientId, int doctorId)
        {                 
            QuestionText = questionText;
            PatientId = patientId;
            DoctorId = doctorId;
        }
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
    }
}
