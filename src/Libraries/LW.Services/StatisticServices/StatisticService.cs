using LW.Data.Entities;
using LW.Data.Repositories.DocumentRepositories;
using LW.Data.Repositories.LessonRepositories;
using LW.Data.Repositories.TopicRepositories;
using LW.Shared.DTOs.Statistic;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LW.Services.StatisticServices;

public class StatisticService : IStatisticService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IDocumentRepository _documentRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly ILessonRepository _lessonRepository;

    public StatisticService(UserManager<ApplicationUser> userManager, IDocumentRepository documentRepository,
        ITopicRepository topicRepository, ILessonRepository lessonRepository, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _documentRepository = documentRepository;
        _topicRepository = topicRepository;
        _lessonRepository = lessonRepository;
        _roleManager = roleManager;
    }

    public async Task<ApiResult<int>> CountUser()
    {
        var countUser = await _userManager.Users.ToListAsync();
        if (!countUser.Any())
        {
            return new ApiResult<int>(false, "Not found User !!!");
        }
        int count = 0;
        count = countUser.Count;
        return new ApiSuccessResult<int>(count);
    }

    public async Task<ApiResult<int>> CountRole()
    {
        var countRole = await _roleManager.Roles.ToListAsync();
        if (!countRole.Any())
        {
            return new ApiResult<int>(false, "Not found Role !!!");
        }
        int count = 0;
        count = countRole.Count;
        return new ApiSuccessResult<int>(count);
    }

    public async Task<ApiResult<IEnumerable<StatisticDto>>> DocumentStatistic(int year)
    {
        var documentList = await _documentRepository.GetAllDocument();
        if (!documentList.Any())
        {
            return new ApiResult<IEnumerable<StatisticDto>>(false, "Not found document !!!");
        }

        var listStatistic = new List<StatisticDto>();
        for (int i = 1; i <= 12; i++)
        {
            var countDocument = documentList.Where(d => d.CreatedDate.Month == i && d.CreatedDate.Year == year);
            listStatistic.Add(new StatisticDto
            {
                Month = i,
                Count = countDocument.Count()
            });
        }
        
        return new ApiSuccessResult<IEnumerable<StatisticDto>>(listStatistic);
    }

    public async Task<ApiResult<IEnumerable<StatisticDto>>> TopicStatistic(int year)
    {
        var topicList = await _topicRepository.GetAllTopic();
        if (!topicList.Any())
        {
            return new ApiResult<IEnumerable<StatisticDto>>(false, "Not found topic !!!");
        }
        
        var listStatistic = new List<StatisticDto>();
        for (int i = 1; i <= 12; i++)
        {
            var countTopic = topicList.Where(d => d.CreatedDate.Month == i && d.CreatedDate.Year == year);
            listStatistic.Add(new StatisticDto
            {
                Month = i,
                Count = countTopic.Count()
            });
        }
        
        return new ApiSuccessResult<IEnumerable<StatisticDto>>(listStatistic);
    }

    public async Task<ApiResult<IEnumerable<StatisticDto>>> LessonStatistic(int year)
    {
        var lessonList = await _lessonRepository.GetAllLesson();
        if (!lessonList.Any())
        {
            return new ApiResult<IEnumerable<StatisticDto>>(false, "Not found lesson !!!");
        }
        
        var listStatistic = new List<StatisticDto>();
        for (int i = 1; i <= 12; i++)
        {
            var countLesson = lessonList.Where(d => d.CreatedDate.Month == i && d.CreatedDate.Year == year);
            listStatistic.Add(new StatisticDto
            {
                Month = i,
                Count = countLesson.Count()
            });
        }
        
        return new ApiSuccessResult<IEnumerable<StatisticDto>>(listStatistic);
    }
}