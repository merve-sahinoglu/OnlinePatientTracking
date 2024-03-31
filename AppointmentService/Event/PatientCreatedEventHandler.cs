using AppointmentService.Data;
using AppointmentService.Domain;
using EventBus.Base;

namespace AppointmentService.Event
{
    public class PatientCreatedEventHandler : IIntegrationEventHandler<PatientCreatedEvent>
    {
        private readonly AppointmentServiceContext _context;
        public PatientCreatedEventHandler(AppointmentServiceContext context)
        {
            _context = context;
        }

        public async Task Handle(PatientCreatedEvent @event)
        {
            var doctor = new Patient()
            {
                Name = @event.Name,
                Surname = @event.Surname,
                Id = @event.Id
            };

            _context.Patient.Add(doctor);
            await _context.SaveChangesAsync();

        }
    }
}
