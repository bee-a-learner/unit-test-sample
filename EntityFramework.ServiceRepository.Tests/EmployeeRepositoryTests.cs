using Bogus;
using EntityFramework.ServiceRepository.DbModel;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace EntityFramework.ServiceRepository.Tests
{
    [TestClass]
    
    public class EmployeeRepositoryTests
    {
        Mock<EmployeeDbContext> mockdbContext;
        IEmployeeRepository repository;

        [TestInitialize]
        public void Init()
        {
            
            

            var stub = GenerateData(10);
            var data = stub.AsQueryable();
            //.AsQueryable();

            //NOTE: always use ()=> in the setup for dbcontext
            var mockSet = new Mock<DbSet<Employee>>();
            mockSet.As<IQueryable<Employee>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Employee>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Employee>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Employee>>().Setup(m => m.GetEnumerator()).Returns(()=>data.GetEnumerator());

            mockdbContext = new Mock<EmployeeDbContext>();
            mockdbContext.Setup(c => c.Employees).Returns(mockSet.Object);
            mockdbContext.Setup(a => a.Set<Employee>()).Returns(mockSet.Object);
            mockdbContext.Setup(a => a.SaveChanges()).Returns(() => 1);

            repository = new EmployeeRepository(mockdbContext.Object);
        }

        [TestMethod()]
        [Priority(1)]
        public async Task get_employee_test()
        {
            var response = await repository.GetAllEmployee();

            response.Count.Should().Be(10);
        }

        [TestMethod()]
        [Priority(2)]
        public async Task add_new_employee_test()
        {
            var employee = GenerateData(1).FirstOrDefault();
            

            var response = await repository.AddEmployee(employee);

            response.Should().BeTrue();

            //mockSet.Verify(m => m.Add(It.IsAny<Blog>()), Times.Once());
            //mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        [Priority(3)]
        public async Task update_an_employee_test()
        {
            var employee = GenerateData(1).FirstOrDefault();


            var response = await repository.AddOrUpdateEmployee(employee);

            response.Should().BeTrue();
        }

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