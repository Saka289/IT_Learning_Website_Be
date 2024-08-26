using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LW.Shared.SeedWork;
using LW.Shared.DTOs.Grade;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.DocumentRepositories;
using LW.Data.Repositories.GradeRepositories;
using LW.Data.Repositories.LessonRepositories;
using LW.Data.Repositories.LevelRepositories;
using LW.Data.Repositories.TopicRepositories;
using LW.Services.GradeServices;
using LW.Shared.DTOs.Document;
using LW.Shared.DTOs.Lesson;
using LW.Shared.DTOs.Topic;
using LW.Shared.Constant;
using LW.Data.Repositories.ExamRepositories;
using LW.Data.Repositories.ProblemRepositories;
using LW.Data.Repositories.QuizRepositories;
using LW.Data.Repositories.SolutionRepositories;
using LW.Shared.DTOs.Exam;
using LW.Shared.DTOs.Problem;
using LW.Shared.DTOs.Quiz;
using LW.Shared.DTOs.Solution;

namespace LW.UnitTests.Service
{


    [TestFixture]
    public class GradeServiceTests
    {
        private IGradeRepository _gradeRepository;
        private ILevelRepository _levelRepository;
        private IElasticSearchService<GradeDto, int> _elasticSearchService;
        private IElasticSearchService<DocumentDto, int> _elasticSearchDocumentService;
        private IElasticSearchService<TopicDto, int> _elasticSearchTopicService;
        private IElasticSearchService<LessonDto, int> _elasticSearchLessonService;
        private IElasticSearchService<ProblemDto, int> _elasticSearchProblemService;
        private IElasticSearchService<QuizDto, int> _elasticSearchQuizService;
        private IElasticSearchService<SolutionDto, int> _elasticSearchSolutionService;
        private IElasticSearchService<ExamDto, int> _elasticSearchExamService;
        private IMapper _mapper;
        private IDocumentRepository _documentRepository;
        private ITopicRepository _topicRepository;
        private ILessonRepository _lessonRepository;
        private IProblemRepository _problemRepository;
        private IQuizRepository _quizRepository;
        private IExamRepository _examRepository;
        private ISolutionRepository _solutionRepository;
        private GradeService _gradeService;

        [SetUp]
        public void Setup()
        {
            _gradeRepository = Substitute.For<IGradeRepository>();
            _levelRepository = Substitute.For<ILevelRepository>();
            _elasticSearchService = Substitute.For<IElasticSearchService<GradeDto, int>>();
            _elasticSearchDocumentService = Substitute.For<IElasticSearchService<DocumentDto, int>>();
            _elasticSearchTopicService = Substitute.For<IElasticSearchService<TopicDto, int>>();
            _elasticSearchLessonService = Substitute.For<IElasticSearchService<LessonDto, int>>();
            _elasticSearchProblemService = Substitute.For<IElasticSearchService<ProblemDto, int>>();
            _elasticSearchQuizService = Substitute.For<IElasticSearchService<QuizDto, int>>();
            _elasticSearchSolutionService = Substitute.For<IElasticSearchService<SolutionDto, int>>();
            _elasticSearchExamService = Substitute.For<IElasticSearchService<ExamDto, int>>();
            _mapper = Substitute.For<IMapper>();
            _documentRepository = Substitute.For<IDocumentRepository>();
            _topicRepository = Substitute.For<ITopicRepository>();
            _lessonRepository = Substitute.For<ILessonRepository>();
            _problemRepository = Substitute.For<IProblemRepository>();
            _quizRepository = Substitute.For<IQuizRepository>();
            _examRepository = Substitute.For<IExamRepository>();
            _solutionRepository = Substitute.For<ISolutionRepository>();

            _gradeService = new GradeService(
                _gradeRepository,
                _mapper,
                _elasticSearchService,
                _elasticSearchDocumentService,
                _elasticSearchTopicService,
                _elasticSearchLessonService,
                _documentRepository,
                _topicRepository,
                _lessonRepository,
                _problemRepository,
                _quizRepository,
                _examRepository,
                _elasticSearchProblemService,
                _elasticSearchQuizService,
                _solutionRepository,
                _elasticSearchSolutionService,
                _elasticSearchExamService
            );
        }

        [Test]
        public async Task GetAllGrade_ShouldReturnGradesSuccessfully()
        {
            // Arrange
            var grades = new List<Grade>
        {
            new Grade { Id = 1, Title = "Grade 1" },
            new Grade { Id = 2, Title = "Grade 2" }
        };
            var gradeDtos = new List<GradeDto>
        {
            new GradeDto { Id = 1, Title = "Grade 1" },
            new GradeDto { Id = 2, Title = "Grade 2" }
        };

            _gradeRepository.GetAllGrade().Returns(Task.FromResult(grades.AsEnumerable()));
            _mapper.Map<IEnumerable<GradeDto>>(grades).Returns(gradeDtos);

            // Act
            var result = await _gradeService.GetAllGrade();

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual(gradeDtos, result.Data);
        }



        [Test]
        public async Task GetGradeById_ShouldReturnGradeSuccessfully_WhenGradeExists()
        {
            // Arrange
            var gradeId = 1;
            var levelId = 1;
            var grade = new Grade { Id = gradeId, Title = "Grade 1", LevelId = levelId };
            var level = new Level { Id = levelId, Title = "Level 1" };
            var gradeDto = new GradeDto { Id = gradeId, Title = "Grade 1", LevelId = levelId };

            _gradeRepository.GetGradeById(gradeId).Returns(Task.FromResult(grade));
            _levelRepository.GetLevelById(levelId).Returns(Task.FromResult(level));
            _mapper.Map<GradeDto>(grade).Returns(gradeDto);

            // Act
            var result = await _gradeService.GetGradeById(gradeId);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual(gradeDto, result.Data);
        }

        [Test]
        public async Task GetGradeById_ShouldReturnNotFound_WhenGradeDoesNotExist()
        {
            // Arrange
            var gradeId = 1;

            _gradeRepository.GetGradeById(gradeId).Returns(Task.FromResult<Grade>(null));

            // Act
            var result = await _gradeService.GetGradeById(gradeId);

            // Assert
            Assert.IsFalse(result.IsSucceeded);
            Assert.IsNull(result.Data);
            Assert.AreEqual("Grade is null !!!", result.Message);
        }

        [Test]
        public async Task CreateGrade_ShouldCreateGradeSuccessfully_WhenLevelExists()
        {
            // Arrange
            var gradeCreateDto = new GradeCreateDto { Title = "New Grade" };
            var level = new Level { Id = 1, Title = "Level 1" };
            var grade = new Grade { Id = 1, Title = "New Grade", LevelId = 1 };
            var gradeDto = new GradeDto { Id = 1, Title = "New Grade" };

            _levelRepository.GetLevelById(level.Id).Returns(Task.FromResult(level));
            _mapper.Map<Grade>(gradeCreateDto).Returns(grade);

            _gradeRepository.CreateGrade(grade).Returns(Task.FromResult(grade)); // Corrected to Task.CompletedTask

            //_gradeRepository.SaveChangesAsync().Returns(Task.CompletedTask);
            _mapper.Map<GradeDto>(grade).Returns(gradeDto);
            _elasticSearchService.CreateDocumentAsync(
                ElasticConstant.ElasticGrades,
                gradeDto,
                Arg.Any<Func<GradeDto, int>>()
            ).Returns(Task.FromResult("DocumentCreated")); // Adjust according to actual return type

            // Act
            var result = await _gradeService.CreateGrade(gradeCreateDto);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual("Success", result.Message);
        }


        [Test]
        public async Task UpdateGrade_ShouldUpdateGradeSuccessfully()
        {
            // Arrange
            var gradeUpdateDto = new GradeUpdateDto { Id = 1, Title = "Updated Grade" };
            var grade = new Grade { Id = 1, Title = "Grade 1", LevelId = 1 };
            var updatedGrade = new Grade { Id = 1, Title = "Updated Grade", LevelId = 1 };
            var gradeDto = new GradeDto { Id = 1, Title = "Updated Grade", LevelId = 1 };
            var documents = new List<Document> { new Document { Id = 1, Title = "Document 1" } };
            var documentDtos = new List<DocumentDto> { new DocumentDto { Id = 1, Title = "Document 1" } };

            // Mock _gradeRepository.GetGradeById()
            _gradeRepository.GetGradeById(gradeUpdateDto.Id, false).Returns(grade);
            _gradeRepository.GetGradeById(gradeUpdateDto.Id, true).Returns(grade);

            // Mock _mapper.Map<GradeDto>(grade)
            _mapper.Map<GradeDto>(grade).Returns(gradeDto);

            // Mock _mapper.Map(gradeUpdateDto, grade)
            _mapper.Map(gradeUpdateDto, grade).Returns(updatedGrade);

            // Mock _gradeRepository.UpdateGrade()
            _gradeRepository.UpdateGrade(updatedGrade).Returns(Task.FromResult(updatedGrade));

            // Mock _gradeRepository.SaveChangesAsync()
            //_gradeRepository.SaveChangesAsync().Returns(Task.CompletedTask);

            // Mock _elasticSearchService.CreateDocumentAsync()
           _elasticSearchService.CreateDocumentAsync(Arg.Any<string>(), Arg.Any<GradeDto>(), Arg.Any<Func<GradeDto, int>>())
                .Returns(Task.FromResult("Some result"));

            // Mock _elasticSearchService.UpdateDocumentAsync()
            _elasticSearchService.UpdateDocumentAsync(Arg.Any<string>(), Arg.Any<GradeDto>(), Arg.Any<int>())
                .Returns(Task.FromResult("Some result"));

            // Mock _documentRepository.GetAllDocumentByGrade()
            _documentRepository.GetAllDocumentByGrade(gradeUpdateDto.Id).Returns(Task.FromResult((IEnumerable<Document>)documents));

            // Mock _mapper.Map<IEnumerable<DocumentDto>>(documents)
            _mapper.Map<IEnumerable<DocumentDto>>(documents).Returns(documentDtos);

            // Mock _elasticSearchDocumentService.UpdateDocumentRangeAsync()
            _elasticSearchDocumentService.UpdateDocumentRangeAsync(Arg.Any<string>(), Arg.Any<IEnumerable<DocumentDto>>(), Arg.Any<Func<DocumentDto, int>>())
                .Returns(Task.FromResult(Enumerable.Empty<string>()));

            // Act
            var result = await _gradeService.UpdateGrade(gradeUpdateDto);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual(null, result.Data);

            // Verifications
            await _elasticSearchService.Received(1).UpdateDocumentAsync(ElasticConstant.ElasticGrades, gradeDto, gradeDto.Id);
        }



        [Test]
        public async Task UpdateGradeStatus_ShouldUpdateGradeStatusSuccessfully()
        {
            // Arrange
            var gradeId = 1;
            var grade = new Grade { Id = gradeId, Title = "Grade 1", LevelId = 1, IsActive = true };
            var level = new Level { Id = 1, Title = "Level 1" };
            var gradeDto = new GradeDto { Id = gradeId, Title = "Grade 1", LevelId = 1, IsActive = false };


            _gradeRepository.GetGradeById(gradeId).Returns(Task.FromResult(grade));
            _levelRepository.GetLevelById(grade.LevelId).Returns(Task.FromResult(level));
            _mapper.Map<GradeDto>(grade).Returns(gradeDto);

            // Act
            var result = await _gradeService.UpdateGradeStatus(gradeId);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.IsTrue(result.Data);
        }

        [Test]
        public async Task DeleteGrade_ShouldDeleteGradeSuccessfully_WhenGradeExists()
        {
            // Arrange
            int gradeId = 1;
            var grade = new Grade { Id = gradeId };
            var documents = new List<Document>
    {
        new Document { Id = 1 },
        new Document { Id = 2 }
    };
            var topics = new List<Topic>
    {
        new Topic { Id = 1 },
        new Topic { Id = 2 }
    };
            var topicChildren = new List<Topic>
    {
        new Topic { Id = 3 },
        new Topic { Id = 4 }
    };
            var lessons = new List<Lesson>
    {
        new Lesson { Id = 1 },
        new Lesson { Id = 2 }
    };

            // Mock return values
            _gradeRepository.GetGradeById(gradeId).Returns(Task.FromResult(grade));
            _documentRepository.GetAllDocumentByGrade(gradeId).Returns(Task.FromResult((IEnumerable<Document>)documents));
            _topicRepository.GetAllTopicByDocument(Arg.Any<int>()).Returns(Task.FromResult((IEnumerable<Topic>)topics));
            _topicRepository.GetAllTopicChild(Arg.Any<int>()).Returns(Task.FromResult((IEnumerable<Topic>)topicChildren));
            _lessonRepository.GetAllLessonByTopic(Arg.Any<int>()).Returns(Task.FromResult((IEnumerable<Lesson>)lessons));

            _elasticSearchDocumentService
                .DeleteDocumentRangeAsync(ElasticConstant.ElasticDocuments, Arg.Any<List<int>>())
                .Returns(Task.FromResult(true)); // Ensure type matches

            _elasticSearchTopicService
                .DeleteDocumentRangeAsync(ElasticConstant.ElasticTopics, Arg.Any<List<int>>())
                .Returns(Task.FromResult(true)); // Ensure type matches

            _elasticSearchLessonService
                .DeleteDocumentRangeAsync(ElasticConstant.ElasticLessons, Arg.Any<List<int>>())
                .Returns(Task.FromResult(true)); // Ensure type matches

            _gradeRepository.DeleteGrade(gradeId).Returns(Task.FromResult(true));
            _elasticSearchService.DeleteDocumentAsync(ElasticConstant.ElasticGrades, gradeId).Returns(Task.FromResult(true));

            // Act
            var result = await _gradeService.DeleteGrade(gradeId);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual("Delete Grade Successfully !!!", result.Message);

            // Verify interactions
            await _gradeRepository.Received().GetGradeById(gradeId);
            await _documentRepository.Received().GetAllDocumentByGrade(gradeId);
            await _elasticSearchDocumentService.Received().DeleteDocumentRangeAsync(
                ElasticConstant.ElasticDocuments, Arg.Any<List<int>>());

            await _gradeRepository.Received().DeleteGrade(gradeId);
            await _elasticSearchService.Received().DeleteDocumentAsync(ElasticConstant.ElasticGrades, gradeId);
        }

    }

}