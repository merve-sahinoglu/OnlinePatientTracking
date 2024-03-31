using EventBus.Base;

namespace DoctorService.Event
{
    public class DoctorCreatedEvent:IntegrationEvent
    {
        public DoctorCreatedEvent(string name, string surname, int deparmantId, int id)
        {
            Name = name;
            Surname = surname;
            DepartmanId = deparmantId;
            Id = id;

        }
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public int DepartmanId { get; private set; }
    }


}
