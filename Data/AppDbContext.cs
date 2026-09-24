using AIHelpdeskAssistant.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AIHelpdeskAssistant.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<SupportRequest> SupportRequests { get; set; }
    }
}