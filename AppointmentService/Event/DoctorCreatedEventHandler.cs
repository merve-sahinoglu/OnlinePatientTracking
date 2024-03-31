using AppointmentService.Data;
using AppointmentService.Domain;
using EventBus.Base;

namespace AppointmentService.Event
{
    public class DoctorCreatedEventHandler : IIntegrationEventHandler<DoctorCreatedEvent>
    {
        private readonly AppointmentServiceContext _context;
        public DoctorCreatedEventHandler(AppointmentServiceContext context)
        {
            _context = context;
        }

        public async Task Handle(DoctorCreatedEvent @event)
        {
            var doctor = new Doctor()
            {
                Name = @event.Name,
                Surname = @event.Surname,
                DepartmanId = @event.DepartmanId,
                Id = @event.Id
            };

            _context.Doctor.Add(doctor);
            await _context.SaveChangesAsync();

        }

    }
}
