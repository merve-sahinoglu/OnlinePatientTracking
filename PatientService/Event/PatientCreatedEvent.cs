using EventBus.Base;

namespace PatientService.Event
{
    public class PatientCreatedEvent : IntegrationEvent
    {
        public PatientCreatedEvent(string name, string surname, int ıd)
        {
            Name = name;
            Surname = surname;
            Id = ıd;

        }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Id { get; set; }
    }



}
