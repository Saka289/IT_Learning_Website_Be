﻿using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.ExamAnswerRepositories;

public interface IExamAnswerRepository : IRepositoryBase<ExamAnswer, int>
{
    Task<IEnumerable<ExamAnswer>> GetAllExamAnswer();
    Task<IEnumerable<ExamAnswer>> GetAllExamAnswerByExamId(int examId);
    Task<ExamAnswer> GetExamAnswerById(int id);
    Task<bool> CreateRangeExamAnswer(IEnumerable<ExamAnswer> examAnswers);
    Task<ExamAnswer> CreateExamAnswer(ExamAnswer examAnswer);
    Task<ExamAnswer> UpdateExamAnswer(ExamAnswer examAnswer);
    Task<bool> DeleteExamAnswer(int id);
    Task<bool> DeleteRangeExamAnswer(IEnumerable<ExamAnswer> examAnswers);
}