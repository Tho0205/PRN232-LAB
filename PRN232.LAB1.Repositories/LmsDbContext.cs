using Bogus;
using Microsoft.EntityFrameworkCore;
using PRN232.LAB1.Repositories.Entities;

namespace PRN232.LAB1.Repositories
{
    public class LmsDbContext : DbContext
    {
        public LmsDbContext(DbContextOptions<LmsDbContext> options) : base(options)
        {
        }

        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            Randomizer.Seed = new Random(123); 

            var semesterIds = 1;
            var semesterFaker = new Faker<Semester>()
                .RuleFor(s => s.SemesterId, f => semesterIds++)
                .RuleFor(s => s.SemesterName, f => f.Date.Past(1).ToString("yyyy") + " " + f.PickRandom("Spring", "Summer", "Fall"))
                .RuleFor(s => s.StartDate, f => f.Date.Past(1))
                .RuleFor(s => s.EndDate, (f, s) => s.StartDate.AddMonths(4));

            var semesters = semesterFaker.Generate(5);
            modelBuilder.Entity<Semester>().HasData(semesters);

            var subjectIds = 1;
            var subjectFaker = new Faker<Subject>()
                .RuleFor(s => s.SubjectId, f => subjectIds++)
                .RuleFor(s => s.SubjectCode, f => f.Random.String2(3, "ABCDEFGHIJKLMNOPQRSTUVWXYZ") + f.Random.Number(100, 999))
                .RuleFor(s => s.SubjectName, f => f.Lorem.Word())
                .RuleFor(s => s.Credit, f => f.Random.Number(1, 4));

            var subjects = subjectFaker.Generate(10);
            modelBuilder.Entity<Subject>().HasData(subjects);

            var studentIds = 1;
            var studentFaker = new Faker<Student>()
                .RuleFor(s => s.StudentId, f => studentIds++)
                .RuleFor(s => s.FullName, f => f.Name.FullName())
                .RuleFor(s => s.Email, (f, s) => f.Internet.Email(s.FullName))
                .RuleFor(s => s.DateOfBirth, f => f.Date.Past(25, DateTime.Now.AddYears(-18)));

            var students = studentFaker.Generate(50);
            modelBuilder.Entity<Student>().HasData(students);

            var courseIds = 1;
            var courseFaker = new Faker<Course>()
                .RuleFor(c => c.CourseId, f => courseIds++)
                .RuleFor(c => c.CourseName, f => "Course " + f.Lorem.Word())
                .RuleFor(c => c.SemesterId, f => f.PickRandom(semesters).SemesterId);

            var courses = courseFaker.Generate(20);
            modelBuilder.Entity<Course>().HasData(courses);

            var enrollmentIds = 1;
            var enrollmentFaker = new Faker<Enrollment>()
                .RuleFor(e => e.EnrollmentId, f => enrollmentIds++)
                .RuleFor(e => e.StudentId, f => f.PickRandom(students).StudentId)
                .RuleFor(e => e.CourseId, f => f.PickRandom(courses).CourseId)
                .RuleFor(e => e.EnrollDate, f => f.Date.Recent(30))
                .RuleFor(e => e.Status, f => f.PickRandom("Active", "Dropped", "Completed"));

            var enrollments = enrollmentFaker.Generate(500);
            modelBuilder.Entity<Enrollment>().HasData(enrollments);
        }
    }
}
