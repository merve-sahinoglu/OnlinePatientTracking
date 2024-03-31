using AppointmentService.Event;

namespace AppointmentService.Extension
{
    public static class EventHandlerRegistration
    {
        public static IServiceCollection ConfigureEventHandlers(this IServiceCollection services)
        {
            services.AddTransient<DoctorCreatedEventHandler>();
            services.AddTransient<PatientCreatedEventHandler>();


            return services;
        }
    }
}
