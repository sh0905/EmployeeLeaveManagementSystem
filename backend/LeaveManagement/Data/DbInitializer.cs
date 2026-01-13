using LeaveManagement.Models;

namespace LeaveManagement.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Check if users already exist
            if (context.Users.Any())
            {
                return; // DB has been seeded
            }

            // Create exactly TWO users: one employee and one manager
            var users = new User[]
            {
                new User
                {
                    Name = "Test Employee",
                    Email = "employee@test.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                    Role = "EMPLOYEE"
                },
                new User
                {
                    Name = "Test Manager",
                    Email = "manager@test.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                    Role = "MANAGER"
                }
            };

            context.Users.AddRange(users);
            context.SaveChanges();
        }
    }
}