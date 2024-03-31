using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NotificationService.Domain;

namespace NotificationService.Data
{
    public class NotificationServiceContext : DbContext
    {
        public NotificationServiceContext (DbContextOptions<NotificationServiceContext> options)
            : base(options)
        {
        }

        public DbSet<NotificationService.Domain.Notification> Notification { get; set; } = default!;
    }
}
