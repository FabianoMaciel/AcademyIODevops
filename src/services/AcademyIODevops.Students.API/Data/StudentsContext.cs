using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AcademyIODevops.Core.Data;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using AcademyIODevops.Students.API.Models;
using AcademyIODevops.Core.Messages;
using System.Diagnostics.CodeAnalysis;

namespace AcademyIODevops.Students.API.Data
{
    [ExcludeFromCodeCoverage]
    public class StudentsContext : DbContext, IUnitOfWork
    {
        public StudentsContext(DbContextOptions<StudentsContext> options) : base(options) { }

        public DbSet<StudentUser> StudentUsers { get; set; }

        public DbSet<Certification> Certifications { get; set; }

        public DbSet<Registration> Registrations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<Event>();

            modelBuilder.ApplyConfiguration(new StudentUserConfiguration());
            modelBuilder.ApplyConfiguration(new RegistrationConfiguration());
            modelBuilder.ApplyConfiguration(new CertificationConfiguration());
        }
        public async Task<bool> Commit()
        {
            foreach (var entry in ChangeTracker.Entries().Where(entry => entry.Entity.GetType().GetProperty("CreatedDate") != null))
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreatedDate").CurrentValue = DateTime.Now;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Property("CreatedDate").IsModified = false;
                }
            }

            return await base.SaveChangesAsync() > 0;
        }
    }

    [ExcludeFromCodeCoverage]
    public class StudentUserConfiguration : IEntityTypeConfiguration<StudentUser>
    {
        public void Configure(EntityTypeBuilder<StudentUser> builder)
        {
            builder.HasKey(a => a.Id);
            builder.ToTable("StudentUsers");
        }
    }

    [ExcludeFromCodeCoverage]
    public class CertificationConfiguration : IEntityTypeConfiguration<Certification>
    {
        public void Configure(EntityTypeBuilder<Certification> builder)
        {
            builder.HasKey(a => a.Id);
            builder.ToTable("Certifications");

            builder.HasOne(ct => ct.Student)
                 .WithMany()
                 .HasForeignKey(ct => ct.StudentId)
                 .OnDelete(DeleteBehavior.Cascade);
        }
    }

    [ExcludeFromCodeCoverage]
    public class RegistrationConfiguration : IEntityTypeConfiguration<Registration>
    {
        public void Configure(EntityTypeBuilder<Registration> builder)
        {
            builder.HasKey(a => a.Id);
            builder.ToTable("Registrations");

            builder.HasOne(r => r.Student)
                 .WithMany()
                 .HasForeignKey(r => r.StudentId)
                 .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
