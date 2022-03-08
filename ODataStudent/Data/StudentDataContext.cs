using Microsoft.EntityFrameworkCore;
using ODataStudent.Models;

namespace ODataStudent.Data
{
    public class StudentDataContext: DbContext
    {
        public StudentDataContext(DbContextOptions<StudentDataContext> options) : base(options)
        {

        }
        public DbSet<Student> Students { get; set; }
    }

}
