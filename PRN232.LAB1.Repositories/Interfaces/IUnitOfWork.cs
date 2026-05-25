namespace PRN232.LAB1.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Entities.Student> StudentRepository { get; }
        IGenericRepository<Entities.Course> CourseRepository { get; }
        IGenericRepository<Entities.Subject> SubjectRepository { get; }
        IGenericRepository<Entities.Semester> SemesterRepository { get; }
        IGenericRepository<Entities.Enrollment> EnrollmentRepository { get; }

        Task SaveAsync();
    }
}
