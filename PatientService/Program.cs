using EventBus.Base;
using EventBus.Factory;
using EventBus.RabbitMQ;
using Microsoft.EntityFrameworkCore;
using PatientService.Data;
using RabbitMQ.Client;
using System.Reflection;

try
{

    var builder = WebApplication.CreateBuilder(args);



    builder.Services.AddDbContext<PatientServiceContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("PatientServiceContext"),
             x => x.MigrationsAssembly("PatientService")
             .MigrationsHistoryTable("__PatientMigrationsHistory"))
             .EnableSensitiveDataLogging();
    });




    // Add services to the container.

    builder.Services.AddControllers();

    builder.Services.AddSingleton<IEventBus>(sp =>
    {
        EventBusConfig config = new()
        {
            ConnectionRetryCount = 5,
            EventNameSuffix = RabbitMQConst.Event,
            SubscriberClientAppName = "Patient",
            DefaultTopicName = Assembly.GetExecutingAssembly().GetName()?.Name,
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
catch (Exception ex)
{

    throw ex;
}
