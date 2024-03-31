using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DoctorService.Domain;

namespace DoctorService.Data
{
    public class DoctorServiceContext : DbContext
    {
        public DoctorServiceContext (DbContextOptions<DoctorServiceContext> options)
            : base(options)
        {
        }

        public DbSet<DoctorService.Domain.Doctor> Doctor { get; set; } = default!;
    }
}
