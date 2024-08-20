using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.UserExamRepositories;

public interface IUserExamRepository : IRepositoryBase<UserExam, int>
{
    Task<UserExam> CreateUserExam(UserExam userExam);
    Task CreateRangeUserExam(IEnumerable<UserExam> userExam);
    Task<UserExam> GetUserExamById(int id);
    Task<IEnumerable<UserExam>> GetAllUserExam();
    Task<IEnumerable<UserExam>> GetAllUserExamByUserId(string userId);
}