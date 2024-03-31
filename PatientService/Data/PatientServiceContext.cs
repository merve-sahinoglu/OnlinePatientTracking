using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PatientService.Domain;

namespace PatientService.Data
{
    public class PatientServiceContext : DbContext
    {
        public PatientServiceContext (DbContextOptions<PatientServiceContext> options)
            : base(options)
        {
        }

        public DbSet<PatientService.Domain.Patient> Patient { get; set; } = default!;
    }
}
