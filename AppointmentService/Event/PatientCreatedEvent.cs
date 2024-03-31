using EventBus.Base;

namespace AppointmentService.Event
{
    public class PatientCreatedEvent : IntegrationEvent
    {
        public string Name { get;  set; }
        public string Surname { get;  set; }
        public int Id { get;  set; }
    }
}
