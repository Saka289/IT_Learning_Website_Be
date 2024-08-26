using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.DocumentRepositories;
using LW.Data.Repositories.LessonRepositories;
using LW.Data.Repositories.ProblemRepositories;
using LW.Data.Repositories.QuizRepositories;
using LW.Data.Repositories.SolutionRepositories;
using LW.Data.Repositories.TagRepositories;
using LW.Data.Repositories.TopicRepositories;
using LW.Infrastructure.Common;
using LW.Services.TopicServices;
using LW.Shared.Constant;
using LW.Shared.DTOs.Lesson;
using LW.Shared.DTOs.Problem;
using LW.Shared.DTOs.Quiz;
using LW.Shared.DTOs.Solution;
using LW.Shared.DTOs.Topic;
using LW.Shared.SeedWork;
using MockQueryable.Moq;
using NSubstitute;
using Serilog;


namespace LW.UnitTests.Service
{
    [TestFixture]
    public class TopicServiceTests
    {
        private ITopicRepository topicRepository;
        private IMapper mapper;
        private IDocumentRepository documentRepository;
        private IElasticSearchService<TopicDto, int> elasticSearchService;
        private ILogger logger;
        private ILessonRepository lessonRepository;
        private IElasticSearchService<LessonDto, int> elasticSearchLessonService;
        private IQuizRepository quizRepository;
        private IElasticSearchService<QuizDto, int> elasticSearchQuizService;
        private IProblemRepository problemRepository;
        private ISolutionRepository solutionRepository;
        private IElasticSearchService<ProblemDto, int> elasticSearchProblemService;
        private IElasticSearchService<SolutionDto, int> elasticSearchSolutionService;
        private ITagRepository tagRepository;
        private TopicService topicService;

        [SetUp]
        public void Setup()
        {
            // Set up mocks for the dependencies
            topicRepository = Substitute.For<ITopicRepository>();
            mapper = Substitute.For<IMapper>();
            documentRepository = Substitute.For<IDocumentRepository>();
            elasticSearchService = Substitute.For<IElasticSearchService<TopicDto, int>>();
            logger = Substitute.For<ILogger>();
            lessonRepository = Substitute.For<ILessonRepository>();
            elasticSearchLessonService = Substitute.For<IElasticSearchService<LessonDto, int>>();
            quizRepository = Substitute.For<IQuizRepository>();
            elasticSearchQuizService = Substitute.For<IElasticSearchService<QuizDto, int>>();
            problemRepository = Substitute.For<IProblemRepository>();
            solutionRepository = Substitute.For<ISolutionRepository>();
            elasticSearchProblemService = Substitute.For<IElasticSearchService<ProblemDto, int>>();
            elasticSearchSolutionService = Substitute.For<IElasticSearchService<SolutionDto, int>>();
            tagRepository = Substitute.For<ITagRepository>();

            // Create an instance of TopicService with the mocked dependencies
            topicService = new TopicService(
                topicRepository,
                mapper,
                documentRepository,
                elasticSearchService,
                logger,
                lessonRepository,
                elasticSearchLessonService,
                quizRepository,
                elasticSearchQuizService,
                problemRepository,
                solutionRepository,
                elasticSearchProblemService,
                elasticSearchSolutionService,
                tagRepository
            );
        }

        [Test]
        public async Task CreateServiceDocumentNotFoundReturnDocumentNotFound()
        {
            // arrange
            TopicCreateDto topicCreateDto = new TopicCreateDto();
            NSubstitute.Core.ConfiguredCall configuredCall =
                documentRepository.GetDocumentById(topicCreateDto.DocumentId).Returns((Document)null);

            // act 
            var result = await topicService.Create(topicCreateDto);
            // assert 
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSucceeded);
            Assert.That(result.Message, Is.EqualTo("Document of topic not found !!!"));
        }

        [Test]
        public async Task CreateShouldCreateTopicSuccessfullyWhenDocumentExists()
        {
            // Arrange
            var model = new TopicCreateDto { DocumentId = 1, Title = "Crème Brûlée", ParentId = null };
            var document = new Document();
            var topic = new Topic { Id = 1, Title = "Creme Brulee" };
            var topicDto = new TopicDto();
            documentRepository.GetDocumentById(model.DocumentId).Returns(document);
            mapper.Map<Topic>(model).Returns(topic);
            topicRepository.CreateTopic(topic).Returns(Task.FromResult(topic));
            topicRepository.GetTopicById(topic.Id).Returns(topic);
            mapper.Map<TopicDto>(topic).Returns(topicDto);
            // Assuming CreateDocumentAsync returns a Task<string>
            elasticSearchService.CreateDocumentAsync(
                ElasticConstant.ElasticTopics,
                topicDto,
                Arg.Any<Func<TopicDto, int>>()
            ).Returns(Task.FromResult("DocumentCreated")); // Adjust according to actual return type


            // Act
            var result = await topicService.Create(model);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual("Create topic successfully", result.Message);
        }

        [Test]
        public async Task CreateShouldUpdateParentTopicWhenParentIdIsProvided()
        {
            // Arrange
            var model = new TopicCreateDto { DocumentId = 1, Title = "Crème Brûlée", ParentId = 2 };
            var document = new Document();
            var topic = new Topic { Id = 1, Title = "Creme Brulee" };
            var parentTopic = new Topic { Id = 2 };
            var topicDto = new TopicDto();

            documentRepository.GetDocumentById(model.DocumentId).Returns(document);
            mapper.Map<Topic>(model).Returns(topic);
            topicRepository.CreateTopic(topic).Returns(Task.FromResult(topic));
            topicRepository.GetTopicById(Convert.ToInt32(model.ParentId)).Returns(parentTopic);
            mapper.Map<TopicDto>(parentTopic).Returns(topicDto);
            // Assuming CreateDocumentAsync returns a Task<string>
            elasticSearchService.CreateDocumentAsync(
                ElasticConstant.ElasticTopics,
                topicDto,
                Arg.Any<Func<TopicDto, int>>()
            ).Returns(Task.FromResult("DocumentCreated")); // Adjust according to actual return type


            // Act
            var result = await topicService.Create(model);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual("Create topic successfully", result.Message);
        }

        [Test]
        public async Task UpdateShouldReturnFalseWhenDocumentNotFound()
        {
            // Arrange
            var model = new TopicUpdateDto { DocumentId = 1, Id = 1, Title = "Crème Brûlée" };
            documentRepository.GetDocumentById(model.DocumentId).Returns(Task.FromResult<Document>(null));

            // Act
            var result = await topicService.Update(model);

            // Assert
            Assert.IsFalse(result.IsSucceeded);
            Assert.AreEqual("Document of topic not found !!!", result.Message);
        }

        [Test]
        public async Task UpdateShouldReturnFalseWhenTopicNotFound()
        {
            // Arrange
            var model = new TopicUpdateDto { DocumentId = 1, Id = 1, Title = "Crème Brûlée" };
            var document = new Document();
            documentRepository.GetDocumentById(model.DocumentId).Returns(Task.FromResult(document));
            topicRepository.GetTopicById(model.Id).Returns(Task.FromResult<Topic>(null));

            // Act
            var result = await topicService.Update(model);

            // Assert
            Assert.IsFalse(result.IsSucceeded);
            Assert.AreEqual("Topic is not found !!!", result.Message);
        }

        [Test]
        public async Task UpdateShouldUpdateTopicAndCreateOrUpdateElasticTopic()
        {
            // Arrange
            var model = new TopicUpdateDto
            {
                DocumentId = 1,
                Id = 1,
                Title = "Updated Title",
                TagValues = new List<string> { "Tag1", "Tag2" },
                ParentId = 2
            };
            var document = new Document();
            var topic = new Topic { Id = 1, Title = "Old Title" };
            var updatedTopic = new Topic { Id = 1, Title = "Updated Title" };
            var topicDto = new TopicDto { Id = 1, Title = "Updated Title" };

            documentRepository.GetDocumentById(model.DocumentId).Returns(document);
            topicRepository.GetTopicById(1).Returns(topic);
            mapper.Map(model, topic).Returns(updatedTopic);
            mapper.Map<TopicDto>(updatedTopic).Returns(topicDto);
            elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticTopics, topicDto, model.ParentId.Value)
                .Returns(Task.FromResult("some String"));

            // Act
            var result = await topicService.Update(model);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual("Update topic successfully", result.Message);
            await topicRepository.Received(1).UpdateTopic(updatedTopic);
            await elasticSearchService.Received(1).UpdateDocumentAsync(
                ElasticConstant.ElasticTopics, topicDto, model.ParentId.Value
            );
        }


        [Test]
        public async Task UpdateStatusShouldReturnFalseWhenTopicNotFound()
        {
            // Arrange
            int topicId = 1;
            topicRepository.GetTopicById(topicId).Returns(Task.FromResult<Topic>(null));

            // Act
            var result = await topicService.UpdateStatus(topicId);

            // Assert
            Assert.IsFalse(result.IsSucceeded);
            Assert.AreEqual("Not found !", result.Message);
        }

        [Test]
        public async Task UpdateStatusShouldUpdateTopicWithParentAndChildren()
        {
            // Arrange
            var topicId = 1;
            var parentId = 2;
            var topic = new Topic { Id = topicId, IsActive = true, ParentId = parentId };
            var updatedTopic = new Topic { Id = topicId, IsActive = false, ParentId = parentId };
            var parentTopic = new Topic { Id = parentId, Title = "Parent Topic" };
            var topicDto = new TopicDto { Id = topicId, IsActive = false };
            var parentTopicDto = new TopicDto { Id = parentId, Title = "Parent Topic" };
            var children = new List<Topic> { new Topic { Id = 3, IsActive = true, ParentId = topicId } };
            var childrenDtos = new List<TopicDto> { new TopicDto { Id = 3, IsActive = false } };

            topicRepository.GetTopicById(topicId).Returns(topic);
            topicRepository.GetAllTopicChild(topicId).Returns(children);
            topicRepository.GetTopicById(parentId).Returns(parentTopic);
            mapper.Map<TopicDto>(topic).Returns(topicDto);
            mapper.Map<TopicDto>(parentTopic).Returns(parentTopicDto);

            // Act
            var result = await topicService.UpdateStatus(topicId);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual("Update status of topic successfully", result.Message);

            await elasticSearchService.Received(1).UpdateDocumentAsync(
                ElasticConstant.ElasticTopics, parentTopicDto, parentId
            );
        }

        [Test]
        public async Task DeleteShouldReturnFalseWhenTopicNotFound()
        {
            // Arrange
            int topicId = 1;
            topicRepository.GetTopicByAllId(topicId).Returns(Task.FromResult<Topic>(null));

            // Act
            var result = await topicService.Delete(topicId);

            // Assert
            Assert.IsFalse(result.IsSucceeded);
            Assert.AreEqual("Not found !", result.Message);
        }

        [Test]
        public async Task DeleteShouldDeleteTopicSuccessfullyWhenNoAssociatedLessons()
        {
            // Arrange
            int topicId = 1;
            var topicEntity = new Topic { Id = topicId };
            var emptyLessonList = new List<Lesson>();

            topicRepository.GetTopicByAllId(topicId).Returns(Task.FromResult(topicEntity));
            lessonRepository.GetAllLessonByTopic(topicId)
                .Returns(Task.FromResult<IEnumerable<Lesson>>(emptyLessonList));
            topicRepository.DeleteTopic(topicId).Returns(Task.FromResult(true));

            // Act
            var result = await topicService.Delete(topicId);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual("Delete topic successfully", result.Message);
        }

        [Test]
        public async Task DeleteShouldDeleteTopicAndAssociatedLessonsSuccessfullyWhenAssociatedLessonsExist()
        {
            // Arrange
            int topicId = 1;
            var topicEntity = new Topic { Id = topicId };
            var lessonList = new List<Lesson> { new Lesson { Id = 1 }, new Lesson { Id = 2 } };
            var lessonDtoList = lessonList.Select(l => new LessonDto { Id = l.Id }).ToList();

            topicRepository.GetTopicByAllId(topicId).Returns(Task.FromResult(topicEntity));
            lessonRepository.GetAllLessonByTopic(topicId).Returns(Task.FromResult<IEnumerable<Lesson>>(lessonList));
            mapper.Map<IEnumerable<LessonDto>>(lessonList).Returns(lessonDtoList);
            topicRepository.DeleteTopic(topicId).Returns(Task.FromResult(true));

            // Act
            var result = await topicService.Delete(topicId);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual("Delete topic successfully", result.Message);
        }

        [Test]
        public async Task GetAllShouldReturnErrorWhenNoTopicsFound()
        {
            // Arrange
            var status = (bool?)null;
            var topics = new List<Topic>();

            topicRepository.GetAllTopic().Returns(Task.FromResult((IEnumerable<Topic>)topics));

            // Act
            var result = await topicService.GetAll(status);

            // Assert
            Assert.IsFalse(result.IsSucceeded);
            Assert.AreEqual("List topic is null !!!", result.Message);
        }


        [Test]
        public async Task GetAllShouldReturnAllTopicsSuccessfullyWhenStatusFilterIsNotApplied()
        {
            // Arrange
            var status = (bool?)null;
            var topics = new List<Topic>
            {
                new Topic { Id = 1, Title = "Topic 1", IsActive = true },
                new Topic { Id = 2, Title = "Topic 2", IsActive = false }
            };
            var topicDtos = new List<TopicDto>
            {
                new TopicDto { Id = 1, Title = "Topic 1", IsActive = true },
                new TopicDto { Id = 2, Title = "Topic 2", IsActive = false }
            };

            topicRepository.GetAllTopic().Returns(Task.FromResult((IEnumerable<Topic>)topics));
            mapper.Map<IEnumerable<TopicDto>>(topics).Returns(topicDtos);

            // Act
            var result = await topicService.GetAll(status);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual("Get all topic successfully !", result.Message);
            Assert.AreEqual(2, result.Data.Count());
        }


        [Test]
        public async Task GetAllTopicByDocumentShouldReturnErrorWhenTopicListIsNull()
        {
            // Arrange
            int documentId = 1;
            var topics = new List<Topic>();

            topicRepository.GetAllTopicByDocument(documentId).Returns(Task.FromResult<IEnumerable<Topic>>(topics));

            // Act
            var result = await topicService.GetAllTopicByDocument(documentId, true);

            // Assert
            Assert.IsFalse(result.IsSucceeded);
            Assert.AreEqual("List topic is null !!!", result.Message);
        }

        [Test]
        public async Task GetAllTopicByDocumentShouldReturnTopicsSuccessfully()
        {
            // Arrange
            int documentId = 1;
            var topics = new List<Topic>
            {
                new Topic { Id = 1, Title = "Topic 1" },
                new Topic { Id = 2, Title = "Topic 2" }
            };

            var topicDtos = new List<TopicDto>
            {
                new TopicDto { Id = 1, Title = "Topic 1" },
                new TopicDto { Id = 2, Title = "Topic 2" }
            };

            topicRepository.GetAllTopicByDocument(documentId).Returns(Task.FromResult<IEnumerable<Topic>>(topics));
            mapper.Map<IEnumerable<TopicDto>>(topics).Returns(topicDtos);

            // Act
            var result = await topicService.GetAllTopicByDocument(documentId, true);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual("Get all topic successfully !", result.Message);
        }

        [Test]
        public async Task GetByIdShouldReturnTopicSuccessfullyWhenTopicExists()
        {
            // Arrange
            var topicId = 1;
            var topic = new Topic { Id = topicId, Title = "Sample Topic" };
            var topicDto = new TopicDto { Id = topicId, Title = "Sample Topic" };

            topicRepository.GetTopicById(topicId).Returns(Task.FromResult(topic));
            mapper.Map<TopicDto>(topic).Returns(topicDto);

            // Act
            var result = await topicService.GetById(topicId);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual(topicDto, result.Data);
            Assert.AreEqual("Get topic by id successfully !", result.Message);
        }

        [Test]
        public async Task GetByIdShouldReturnNotFoundWhenTopicDoesNotExist()
        {
            // Arrange
            var topicId = 1;

            topicRepository.GetTopicById(topicId).Returns(Task.FromResult<Topic>(null));

            // Act
            var result = await topicService.GetById(topicId);

            // Assert
            Assert.IsFalse(result.IsSucceeded);
            Assert.IsNull(result.Data);
            Assert.AreEqual("Not found !", result.Message);
        }

        [Test]
        public async Task GetAllTopicPaginationShouldReturnErrorWhenNoTopicsAreFound()
        {
            // Arrange
            var searchTopicDto = new SearchTopicDto
            {
                Value = "",
                Status = null,
                DocumentId = 0,
                PageIndex = 1,
                PageSize = 10,
                OrderBy = "Title",
                IsAscending = true
            };

            topicRepository.GetAllTopic().Returns(Task.FromResult(Enumerable.Empty<Topic>()));

            // Act
            var result = await topicService.GetAllTopicPagination(searchTopicDto);

            // Assert
            Assert.IsFalse(result.IsSucceeded);
            Assert.AreEqual("Topic is null !!!", result.Message);
        }

        [Test]
        public async Task GetAllTopicPaginationShouldReturnErrorWhenSearchReturnsNoResults()
        {
            // Arrange
            var searchTopicDto = new SearchTopicDto
            {
                Value = "Search Value",
                Status = null,
                DocumentId = 0,
                PageIndex = 1,
                PageSize = 10,
                OrderBy = "Title",
                IsAscending = true
            };

            elasticSearchService.SearchDocumentFieldAsync(ElasticConstant.ElasticTopics, Arg.Any<SearchRequestValue>())
                .Returns(Task.FromResult<IEnumerable<TopicDto>>(null));

            // Act
            var result = await topicService.GetAllTopicPagination(searchTopicDto);

            // Assert
            Assert.IsFalse(result.IsSucceeded);
            Assert.AreEqual("Topic not found !!!", result.Message);
        }
    }
}