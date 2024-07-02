using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.UserGradeRepositories;

public interface IUserGradeRepository:IRepositoryBase<UserGrade,int>
{
    Task CreateUserGrade(UserGrade userGrade);
    Task CreateRangeUserGrade(IEnumerable<UserGrade> userGrades);
    Task UpdateUserGrade(UserGrade level);
    Task<bool> DeleteUserGrade(int id);
    Task<bool> DeleteRangeUserGrade(IEnumerable<UserGrade> userGrades);
    Task<UserGrade> GetUserGradeById(int id);
    Task<IEnumerable<UserGrade>> GetAllUserGrade();
}