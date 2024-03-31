using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuestionAnswerService.Domain;

namespace QuestionAnswerService.Data
{
    public class QuestionAnswerServiceContext : DbContext
    {
        public QuestionAnswerServiceContext (DbContextOptions<QuestionAnswerServiceContext> options)
            : base(options)
        {
        }

        public DbSet<QuestionAnswerService.Domain.Answer> Answer { get; set; } = default!;
        public DbSet<QuestionAnswerService.Domain.Question> Question { get; set; } = default!;
    }
}
