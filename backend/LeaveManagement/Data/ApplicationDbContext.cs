using LeaveManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace LeaveManagement.Data
{
    
    public class ApplicationDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("data source=IBM-PF3PG9ST\\MSSQLSERVER01; initial catalog=LeavemanagementDB;integrated security=True;TrustServerCertificate=true");
        }
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
    }
}
