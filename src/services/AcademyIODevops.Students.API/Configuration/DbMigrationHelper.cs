using AcademyIODevops.Core.Utils;
using AcademyIODevops.Students.API.Data;
using AcademyIODevops.Students.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace AcademyIODevops.Students.API.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class DbMigrationHelperExtension
    {
        public static async Task UseDbMigrationHelperAsync(this WebApplication app)
        {
            await DbMigrationHelper.EnsureSeedData(app);
        }
    }

    [ExcludeFromCodeCoverage]
    public static class DbMigrationHelper
    {
        public static async Task EnsureSeedData(WebApplication application)
        {
            var services = application.Services.CreateScope().ServiceProvider;
            await EnsureSeedData(services);
        }

        public static async Task EnsureSeedData(IServiceProvider serviceProvider)
        {
            var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

            var studentsContext = scope.ServiceProvider.GetRequiredService<StudentsContext>();

            await studentsContext.Database.MigrateAsync();
            await EnsureSeedData(studentsContext);
        }

        private static async Task EnsureSeedData(StudentsContext studentsContext)
        {
            await SeedStudentUsers(studentsContext);
        }

        public static async Task SeedStudentUsers(StudentsContext context)
        {
            if (context.StudentUsers.Any()) return;

            var admin = SeedStudentUserData.Users.FirstOrDefault(a => a.IsAdmin)!;
            var student1 = SeedStudentUserData.Users.FirstOrDefault(a => a.FirstName.Equals("Student1"))!;
            var student2 = SeedStudentUserData.Users.FirstOrDefault(a => a.FirstName.Equals("Student2"))!;

            var studentUserAdmin = new StudentUser(admin.Id, admin.Email, admin.FirstName, admin.LastName, admin.Email, admin.DateOfBirth, admin.IsAdmin);
            await context.StudentUsers.AddAsync(studentUserAdmin);

            var studentUser1 = new StudentUser(student1.Id, student1.Email, student1.FirstName, student1.LastName, student1.Email, student1.DateOfBirth, student1.IsAdmin);
            await context.StudentUsers.AddAsync(studentUser1);

            var studentUser2 = new StudentUser(student2.Id, student2.Email, student2.FirstName, student2.LastName, student2.Email, student2.DateOfBirth, student2.IsAdmin);
            await context.StudentUsers.AddAsync(studentUser2);

            context.SaveChanges();

        }


    }
}
