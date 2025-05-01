using Microsoft.EntityFrameworkCore;
using pro1.Models;

namespace pro1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<MCQQuestion> MCQQuestions { get; set; }
        public DbSet<Exams> Exams { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<Audit> Audits { get; set; }
        public DbSet<SubjectUnit> SubjectUnits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Exams.CreatedAt default value
            modelBuilder.Entity<Exams>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            // ExamQuestion relationships
            modelBuilder.Entity<ExamQuestion>()
                .HasOne(eq => eq.Exam)
                .WithMany(e => e.ExamQuestions)
                .HasForeignKey(eq => eq.ExamID)
                .OnDelete(DeleteBehavior.Cascade); // Deleting an exam deletes its exam questions

            modelBuilder.Entity<ExamQuestion>()
                .HasOne(eq => eq.McqQuestion)
                .WithMany(q => q.ExamQuestions)
                .HasForeignKey(eq => eq.QuestionID)
                .OnDelete(DeleteBehavior.Restrict); // Don't delete questions if they are used

            // Exams → SubjectUnit
            modelBuilder.Entity<Exams>()
                .HasOne(e => e.SubjectUnit)
                .WithMany()
                .HasForeignKey(e => e.SubjectUnitID)
                .OnDelete(DeleteBehavior.Restrict);

            // Exams → Faculty (User)
            modelBuilder.Entity<Exams>()
                .HasOne(e => e.Faculty)
                .WithMany(u => u.Exams)
                .HasForeignKey(e => e.FacultyID)
                .OnDelete(DeleteBehavior.Restrict);

            // MCQQuestion → SubjectUnit
            modelBuilder.Entity<MCQQuestion>()
                .HasOne(q => q.SubjectUnit)
                .WithMany(su => su.MCQQuestions)
                .HasForeignKey(q => q.SubjectUnitID)
                .OnDelete(DeleteBehavior.Restrict);

            // SubjectUnit → CreatedByUser
            modelBuilder.Entity<SubjectUnit>()
                .HasOne(su => su.CreatedByUser)
                .WithMany()
                .HasForeignKey(su => su.CreatedByUserID)
                .OnDelete(DeleteBehavior.Restrict);

            // Audit → User
            modelBuilder.Entity<Audit>()
                .HasOne(a => a.User)
                .WithMany(u => u.Audits)
                .HasForeignKey(a => a.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique constraint: SubjectName + UnitName
            modelBuilder.Entity<SubjectUnit>()
                .HasIndex(su => new { su.SubjectName, su.UnitName })
                .IsUnique();
        }
    }
}
