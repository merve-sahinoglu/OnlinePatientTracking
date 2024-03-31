namespace DoctorService.Domain
{
    public class Doctor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int DepartmanId { get; set; }
    }
}
