using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DoctorService.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DoctorServiceContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DoctorServiceContext") ?? throw new InvalidOperationException("Connection string 'DoctorServiceContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();
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


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DoctorServiceContext>();
    db.Database.Migrate();
}
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
