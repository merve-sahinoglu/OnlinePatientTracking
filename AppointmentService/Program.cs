using Microsoft.EntityFrameworkCore;
using AppointmentService.Data;
using EventBus.Base;
using System.Reflection;
using AppointmentService.Extension;
using RabbitMQ.Client;
using EventBus.RabbitMQ;
using EventBus.Factory;
using AppointmentService.Event;


try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddDbContext<AppointmentServiceContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("AppointmentServiceContext") ?? throw new InvalidOperationException("Connection string 'AppointmentServiceContext' not found.")));

    // Add services to the container.

    builder.Services.AddControllers();

    builder.Services.ConfigureEventHandlers();

    builder.Services.AddSingleton<IEventBus>(sp =>
    {
        EventBusConfig config = new()
        {
            ConnectionRetryCount = 5,
            EventNameSuffix = RabbitMQConst.Event,
            SubscriberClientAppName = "Appoinment",
            DefaultTopicName = Assembly.GetExecutingAssembly().GetName().Name,
            EventBusType = EventBusTypes.RabbitMQ,
            Connection = new ConnectionFactory()
            {
                HostName = builder.Configuration[RabbitMQConst.RabbitMQHostName],
                VirtualHost = builder.Configuration[RabbitMQConst.RabbitMQVirtualHost],
                Port = Convert.ToInt32(builder.Configuration[RabbitMQConst.RabbitMQPort]),
                UserName = builder.Configuration[RabbitMQConst.RabbitMQUserName],
                Password = builder.Configuration[RabbitMQConst.RabbitMQPassword],
            }
        };

        return EventBusFactory.Create(config, sp);
    });

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception)
{

    throw;
}

void AddEventSubscriptions(WebApplication app)
{
    var eventBus = app.Services.GetRequiredService<IEventBus>();

    eventBus.Subscribe<PatientCreatedEvent, PatientCreatedEventHandler>("PatientService");
    eventBus.Subscribe<DoctorCreatedEvent, DoctorCreatedEventHandler>("DoctorService");
}