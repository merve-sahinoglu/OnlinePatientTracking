using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppointmentService.Domain;

namespace AppointmentService.Data
{
    public class AppointmentServiceContext : DbContext
    {
        public AppointmentServiceContext (DbContextOptions<AppointmentServiceContext> options)
            : base(options)
        {
        }

        public DbSet<AppointmentService.Domain.Appointment> Appointment { get; set; } = default!;
        public DbSet<AppointmentService.Domain.Doctor> Doctor { get; set; } = default!;
        public DbSet<AppointmentService.Domain.Patient> Patient { get; set; } = default!;


    }
}
