using EntityFramework.ServiceRepository.DbModel;

namespace EntityFramework.ServiceRepository
{
    public interface IEmployeeRepository
    {
        Task<bool> AddEmployee(Employee record);
        Task<bool> AddOrUpdateEmployee(Employee record);
        Task<List<Employee>> GetAllEmployee();
        Task<bool>  RemoveEmployee(Employee record);
    }
}