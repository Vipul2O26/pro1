// ViewModels/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using pro1.Models;

namespace pro1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<MCQQuestion> MCQQuestions { get; set; }
        public DbSet<Exams> Exams { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<Audit> Audits { get; set; }
        public DbSet<SubjectUnit> SubjectUnits { get; set; }





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Set default value for CreatedAt column in Exams
            modelBuilder.Entity<Exams>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            // Relationship: Exams → ExamQuestions (Cascade delete)
            modelBuilder.Entity<ExamQuestion>()
                .HasOne(eq => eq.Exam)
                .WithMany(e => e.ExamQuestions)
                .HasForeignKey(eq => eq.ExamID)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship: MCQQuestion → ExamQuestion (Restrict delete to preserve used questions)
            modelBuilder.Entity<ExamQuestion>()
                .HasOne(eq => eq.McqQuestion)
                .WithMany(q => q.ExamQuestions)
                .HasForeignKey(eq => eq.QuestionID)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Unique constraint on SubjectName + UnitName in SubjectUnit table
            modelBuilder.Entity<SubjectUnit>()
      .HasIndex(su => new { su.SubjectName, su.UnitName })
      .IsUnique();

        }
    }
}