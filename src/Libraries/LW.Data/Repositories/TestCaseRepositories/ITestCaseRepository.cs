using System.Collections;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.TestCaseRepositories;

public interface ITestCaseRepository : IRepositoryBase<TestCase, int>
{
    Task<IEnumerable<TestCase>> GetAllTestCase();
    Task<IQueryable<TestCase>> GetAllTestCasePagination();
    Task<IEnumerable<TestCase>> GetAllTestCaseByProblemId(int problemId);
    Task<TestCase?> GetTestCaseById(int id);
    Task<TestCase> CreateTestCase(TestCase testCase);
    Task<TestCase> UpdateTestCase(TestCase testCase);
    Task<bool> DeleteTestCase(int id);
    Task<bool> CreateRangeTestCase(IEnumerable<TestCase> testCases);
    Task<bool> UpdateRangeTestCase(IEnumerable<TestCase> testCases);
    Task<bool> DeleteRangeTestCase(IEnumerable<TestCase> testCases);
}