using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.TestCaseRepositories;

public class TestCaseRepository : RepositoryBase<TestCase, int>, ITestCaseRepository
{
    public TestCaseRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IEnumerable<TestCase>> GetAllTestCase()
    {
        return await FindAll().ToListAsync();
    }

    public Task<IQueryable<TestCase>> GetAllTestCasePagination()
    {
        var result = FindAll().AsQueryable();
        return Task.FromResult(result);
    }

    public async Task<IEnumerable<TestCase>> GetAllTestCaseByProblemId(int problemId)
    {
        return await FindAll().Where(t => t.ProblemId == problemId).ToListAsync();
    }

    public async Task<TestCase?> GetTestCaseById(int id)
    {
        return await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<TestCase> CreateTestCase(TestCase testCase)
    {
        await CreateAsync(testCase);
        return testCase;
    }

    public async Task<TestCase> UpdateTestCase(TestCase testCase)
    {
        await UpdateAsync(testCase);
        return testCase;
    }

    public async Task<bool> DeleteTestCase(int id)
    {
        var testCase = await GetByIdAsync(id);
        if (testCase == null)
        {
            return false;
        }

        await DeleteAsync(testCase);
        return true;
    }

    public async Task<bool> CreateRangeTestCase(IEnumerable<TestCase> testCases)
    {
        testCases = testCases.Where(t => t != null);
        if (!testCases.Any())
        {
            return false;
        }

        await CreateListAsync(testCases);
        return true;
    }

    public async Task<bool> UpdateRangeTestCase(IEnumerable<TestCase> testCases)
    {
        testCases = testCases.Where(t => t != null);
        if (!testCases.Any())
        {
            return false;
        }

        await UpdateListAsync(testCases);
        return true;
    }

    public async Task<bool> DeleteRangeTestCase(IEnumerable<TestCase> testCases)
    {
        testCases = testCases.Where(l => l != null);
        if (!testCases.Any())
        {
            return false;
        }

        await DeleteListAsync(testCases);
        return true;
    }
}