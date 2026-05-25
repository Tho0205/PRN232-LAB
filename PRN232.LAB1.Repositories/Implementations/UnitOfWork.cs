using PRN232.LAB1.Repositories.Interfaces;

namespace PRN232.LAB1.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LmsDbContext _context;
        private IGenericRepository<Entities.Student>? _studentRepository;
        private IGenericRepository<Entities.Course>? _courseRepository;
        private IGenericRepository<Entities.Subject>? _subjectRepository;
        private IGenericRepository<Entities.Semester>? _semesterRepository;
        private IGenericRepository<Entities.Enrollment>? _enrollmentRepository;

        public UnitOfWork(LmsDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<Entities.Student> StudentRepository
            => _studentRepository ??= new GenericRepository<Entities.Student>(_context);

        public IGenericRepository<Entities.Course> CourseRepository
            => _courseRepository ??= new GenericRepository<Entities.Course>(_context);

        public IGenericRepository<Entities.Subject> SubjectRepository
            => _subjectRepository ??= new GenericRepository<Entities.Subject>(_context);

        public IGenericRepository<Entities.Semester> SemesterRepository
            => _semesterRepository ??= new GenericRepository<Entities.Semester>(_context);

        public IGenericRepository<Entities.Enrollment> EnrollmentRepository
            => _enrollmentRepository ??= new GenericRepository<Entities.Enrollment>(_context);

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
