using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TicketBackend.Models;

namespace TicketBackend.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(
        TicketDbContext context)
    {
        await context.Database.MigrateAsync();

        // ========================================================
        // Password Hasher
        // ========================================================

        var passwordHasher = new PasswordHasher<User>();

        // ========================================================
        // Categories
        // ========================================================

        if (!await context.Categories.AnyAsync())
        {
            context.Categories.AddRange(
                new Category
                {
                    Name = "Server",
                    Description =
                        "Server and operating system issues"
                },
                new Category
                {
                    Name = "Network",
                    Description =
                        "Network connectivity and infrastructure issues"
                },
                new Category
                {
                    Name = "Application",
                    Description =
                        "Application related issues"
                },
                new Category
                {
                    Name = "Database",
                    Description =
                        "Database related issues"
                },
                new Category
                {
                    Name = "Access",
                    Description =
                        "User access and permission issues"
                }
            );

            await context.SaveChangesAsync();
        }

        // ========================================================
        // Priorities
        // ========================================================

        if (!await context.Priorities.AnyAsync())
        {
            context.Priorities.AddRange(
                new Priority
                {
                    Name = "Low",
                    Level = 1
                },
                new Priority
                {
                    Name = "Medium",
                    Level = 2
                },
                new Priority
                {
                    Name = "High",
                    Level = 3
                },
                new Priority
                {
                    Name = "Critical",
                    Level = 4
                }
            );

            await context.SaveChangesAsync();
        }

        // ========================================================
        // Statuses
        // ========================================================

        if (!await context.TicketStatuses.AnyAsync())
        {
            context.TicketStatuses.AddRange(
                new TicketStatus
                {
                    Name = "Open"
                },
                new TicketStatus
                {
                    Name = "Assigned"
                },
                new TicketStatus
                {
                    Name = "In Progress"
                },
                new TicketStatus
                {
                    Name = "Pending"
                },
                new TicketStatus
                {
                    Name = "Resolved"
                },
                new TicketStatus
                {
                    Name = "Closed"
                }
            );

            await context.SaveChangesAsync();
        }

        // ========================================================
        // Users
        // ========================================================

        if (!await context.Users.AnyAsync())
        {
            var amit = new User
            {
                Username = "amit",
                FullName = "Amit Kumar",
                Email = "amit@example.com",
                Role = "Admin",
                IsActive = true
            };

            amit.PasswordHash =
                passwordHasher.HashPassword(
                    amit,
                    "Amit@123");

            var support = new User
            {
                Username = "support",
                FullName = "Support User",
                Email = "support@example.com",
                Role = "Agent",
                IsActive = true
            };

            support.PasswordHash =
                passwordHasher.HashPassword(
                    support,
                    "Support@123");

            var testuser = new User
            {
                Username = "testuser",
                FullName = "Test User",
                Email = "test@example.com",
                Role = "User",
                IsActive = true
            };

            testuser.PasswordHash =
                passwordHasher.HashPassword(
                    testuser,
                    "Test@123");

            context.Users.AddRange(
                amit,
                support,
                testuser);

            await context.SaveChangesAsync();
        }
        else
        {
            // ====================================================
            // Upgrade existing TEMP_PASSWORD users
            // ====================================================

            var users = await context.Users.ToListAsync();

            foreach (var user in users)
            {
                if (user.PasswordHash == "TEMP_PASSWORD")
                {
                    string password = user.Username.ToLower() switch
                    {
                        "amit" => "Amit@123",
                        "support" => "Support@123",
                        "testuser" => "Test@123",
                        _ => string.Empty
                    };

                    if (!string.IsNullOrEmpty(password))
                    {
                        user.PasswordHash =
                            passwordHasher.HashPassword(
                                user,
                                password);
                    }
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
