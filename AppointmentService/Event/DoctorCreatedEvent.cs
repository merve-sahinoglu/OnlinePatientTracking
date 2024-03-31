using EventBus.Base;

namespace AppointmentService.Event
{
    public class DoctorCreatedEvent : IntegrationEvent
    {

        public string Name { get; set; }
        public string Surname { get;  set; }
        public int DepartmanId { get;  set; }
        public int Id { get;  set; }
    }
}
