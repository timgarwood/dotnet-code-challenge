using CodeChallenge.Models;
using System;
using System.Threading.Tasks;

namespace CodeChallenge.Repositories
{
    public interface IEmployeeRepository
    {
        Employee GetById(String id);

        Employee GetByIdWithDirectReports(string id);

        Employee Add(Employee employee);
        Employee Remove(Employee employee);

        Compensation GetCompensationById(string id);

        Compensation Create(CompensationCreateModel model);

        Task SaveAsync();
    }
}