using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.ExamImageRepositories;

public interface IExamImageRepository : IRepositoryBase<ExamImage,int>
{
    Task<IEnumerable<ExamImage>> GetAllExamImage();
    Task<IEnumerable<ExamImage>> GetAllExamImageByExamId(int examId);
    Task<ExamImage> GetExamImageById(int id);
    Task<bool> CreateRangeExamImage(IEnumerable<ExamImage> examImages);
    Task<ExamImage> CreateExamImage(ExamImage e);
    Task<ExamImage> UpdateExamImage(ExamImage e);
    Task<bool> DeleteExamImage(int id);
    Task<bool> DeleteRangeExamImage(IEnumerable<ExamImage> examImages);
}