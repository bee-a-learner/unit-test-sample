using EntityFramework.ServiceRepository.DbModel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace EntityFramework.ServiceRepository
{
    public class EmployeeDbContext : DbContext
    {
        public EmployeeDbContext() { }

        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options): base(options){ }

        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Department> Departments { get; set; }

    }

}