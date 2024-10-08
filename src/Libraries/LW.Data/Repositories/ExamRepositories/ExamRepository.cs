﻿using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using LW.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.ExamRepositories;

public class ExamRepository : RepositoryBase<Exam, int>, IExamRepository
{
    public ExamRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IEnumerable<Exam>> GetAllExam()
    {
        return await FindAll()
            .Include(x => x.Competition)
            .Include(x => x.Level)
            .Include(x => x.Grade)
            .ToListAsync();
    }

    public async Task<IEnumerable<Exam>> GetAllExamByGrade(int gradeId)
    {
        return await FindAll().Where(x => x.GradeId == gradeId).Include(x => x.Level)
            .Include(x => x.Grade).ToListAsync();
    }

    public async Task<IEnumerable<Exam>> SearchExamByTag(string tag, bool order)
    {
        var result = order
            ? await FindAll()
                .Include(e => e.Competition)
                .Where(e => e.KeyWord.Contains(tag)).OrderByDescending(e => e.CreatedDate).ToListAsync()
            : await FindAll()
                .Include(e => e.Competition)
                .Where(e => e.KeyWord.Contains(tag)).ToListAsync();
        return result;
    }

    public async Task<Exam?> GetExamById(int id)
    {
        return await FindByCondition(x => x.Id == id).Include(x => x.Competition)
            .Include(x => x.ExamCodes)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Exam>> GetExamByType(EExamType type)
    {
        return await FindByCondition(x => x.Type == type).Include(x => x.Competition)
            .Include(x => x.ExamCodes).Include(x => x.Level)
            .Include(x => x.Grade).ToListAsync();
    }

    public async Task<IEnumerable<Exam>> GetExamByCompetitionId(int competitionId)
    {
        return await FindByCondition(x => x.CompetitionId == competitionId)
            .Include(x => x.Competition)
            .Include(x => x.Level)
            .Include(x => x.Grade)
            .ToListAsync();
    }

    public async Task<Exam> CreateExam(Exam e)
    {
        await CreateAsync(e);
        return e;
    }

    public async Task<Exam> UpdateExam(Exam e)
    {
        await UpdateAsync(e);
        return e;
    }

    public async Task<bool> DeleteExam(int id)
    {
        var exam = await GetByIdAsync(id);
        if (exam == null)
        {
            return false;
        }

        await DeleteAsync(exam);
        return true;
    }
}