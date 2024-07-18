﻿using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.UserExamRepositories;

public class UserExamRepository : RepositoryBase<UserExam,int>, IUserExamRepository
{
    public UserExamRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public  Task<UserExam> CreateUserExam(UserExam userExam)
    { 
        CreateAsync(userExam);
       return Task.FromResult(userExam);
    }

    public async Task CreateRangeUserExam(IEnumerable<UserExam> userExams)
    {
        await CreateListAsync(userExams);
    }

    public async Task<UserExam> GetUserExamById(int id)
    {
        return await FindByCondition(x => x.Id == id).Include(x=>x.ApplicationUser).Include(x=>x.ExamCode).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<UserExam>> GetAllUserExam()
    {
        return await FindAll().ToListAsync();
    }

    public async Task<IEnumerable<UserExam>> GetAllUserExamByUserId(string userId)
    {
        return await FindByCondition(x => x.UserId == userId).Include(x=>x.ApplicationUser).Include(x=>x.ExamCode).ToListAsync();
    }
}