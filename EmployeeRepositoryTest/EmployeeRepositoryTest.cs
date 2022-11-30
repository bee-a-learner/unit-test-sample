using Bogus;
using Bogus.DataSets;
using EntityFramework.ServiceRepository;
using EntityFramework.ServiceRepository.DbModel;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeRepositoryTest
{
    [TestClass]
    public class EmployeeRepositoryTest
    {
        List<Employee> stub;
        IEmployeeRepository repository;

        [TestInitialize]
        public async Task Init()
        {
             stub = GenerateData(10);
            var data = stub.AsQueryable();
            var mockdbContext = new Mock<EmployeeDbContext>();
            var mockSet = new Mock<DbSet<Employee>>();
            mockSet.As<IQueryable<Employee>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Employee>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Employee>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Employee>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            mockdbContext.Setup(x => x.Employees).Returns(mockSet.Object);

            mockdbContext.Setup(x => x.SaveChanges()).Returns(1);

            repository = new EmployeeRepository(mockdbContext.Object);

        }

        [TestMethod]
        [Priority(1)]
        public async Task add_new_employee_test()
        {
            var employee = GenerateData(1);
            var response =  await repository.AddEmployee(employee.FirstOrDefault());

            response.Should().BeTrue();

        }


        [TestMethod]
        [Priority(2)]
        public async Task GetAllEmployee_test()
        {
            var employee = GenerateData(1);
            var response = await repository.GetAllEmployee();

            response.Count.Should().Be(10);

        }


        [TestMethod]
        
        public async Task AddOrUpdateEmployee_add_new_test()
        {
            var employee = GenerateData(1).FirstOrDefault();
            var response = await repository.AddOrUpdateEmployee(employee);

            response.Should().BeTrue();

        }

        [TestMethod]

        public async Task AddOrUpdateEmployee_update_test()
        {
            var employee = stub.FirstOrDefault();
            var response = await repository.AddOrUpdateEmployee(employee);

            response.Should().BeTrue();

        }

        [TestMethod]

        public async Task RemoveEmployee_test()
        {
            var employee = stub.FirstOrDefault();
            var response = await repository.RemoveEmployee(employee);

            response.Should().BeTrue();

        }

        //Task<bool> AddEmployee(Employee record);
        //Task<bool> AddOrUpdateEmployee(Employee record);
        //Task<List<Employee>> GetAllEmployee();
        //Task<bool> RemoveEmployee(Employee record);

        private List<Employee> GenerateData(int count)
        {
            var faker = new Faker<Employee>()
                        .RuleFor(c => c.first_name, f => f.Person.FirstName)
                        .RuleFor(c => c.last_name, f => f.Person.LastName)
                        .RuleFor(c => c.desination, f => f.Commerce.Department())
                        .RuleFor(c => c.date_of_joining, f => f.Date.Past())
                         .RuleFor(c => c.status, f => "ACTIVE")
                         .RuleFor(c => c.employee_Id, Guid.NewGuid());

            return faker.Generate(count);

        }
    }
}