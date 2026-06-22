namespace PRN232.LAB.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Entities.Student> StudentRepository { get; }
        IGenericRepository<Entities.Course> CourseRepository { get; }
        IGenericRepository<Entities.Subject> SubjectRepository { get; }
        IGenericRepository<Entities.Semester> SemesterRepository { get; }
        IGenericRepository<Entities.Enrollment> EnrollmentRepository { get; }
        IGenericRepository<Entities.User> UserRepository { get; }
        IGenericRepository<Entities.RefreshToken> RefreshTokenRepository { get; }

        Task SaveAsync();
    }
}
