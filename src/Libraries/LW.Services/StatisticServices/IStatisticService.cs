using LW.Shared.DTOs.Statistic;
using LW.Shared.SeedWork;

namespace LW.Services.StatisticServices;

public interface IStatisticService
{
    Task<ApiResult<int>> CountUser();
    Task<ApiResult<IEnumerable<StatisticRoleDto>>> CountUserByRole();
    Task<ApiResult<IEnumerable<StatisticDto>>> DocumentStatistic(int year);
    Task<ApiResult<IEnumerable<StatisticDto>>> TopicStatistic(int year);
    Task<ApiResult<IEnumerable<StatisticDto>>> LessonStatistic(int year);
}