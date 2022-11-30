using EntityFramework.ServiceRepository.DbModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.ServiceRepository
{
    public class EmployeeRepository : IEmployeeRepository
    {
       readonly EmployeeDbContext _dbContext;
        public EmployeeRepository(EmployeeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<Employee>> GetAllEmployee()
        {
            var query = from b in _dbContext.Employees
                        orderby b.date_of_joining
                        select b;
            return Task.FromResult( query.ToList());
        }

        async Task<bool> IEmployeeRepository.AddEmployee(Employee record)
        {

             _dbContext.Employees.Add(record);
          
            return _dbContext.SaveChanges() > 0;
        }


        async Task<bool> IEmployeeRepository.AddOrUpdateEmployee(Employee record)
        {

            var response = from row in _dbContext.Employees
                           where row.employee_Id == record.employee_Id
                           select row;

            if (response.Any())
            {

                var currentRecord = response.FirstOrDefault();
            
                currentRecord.first_name = record.first_name;
                currentRecord.last_name = record.last_name;
                currentRecord.date_of_joining = record.date_of_joining;
                currentRecord.status = record.status;
                currentRecord.desination = record.desination;
                //    base.Update(currentRecord);
            }
            else
            {
                _dbContext.Employees.Add(record);
            }

            return _dbContext.SaveChanges()>0;
        }

        async Task<bool> IEmployeeRepository.RemoveEmployee(Employee record)
        {
            var response = _dbContext.Employees.Find(record.employee_Id);
            _dbContext.Employees.Remove(response);

            return _dbContext.SaveChanges() > 0;
        }
    }
}
