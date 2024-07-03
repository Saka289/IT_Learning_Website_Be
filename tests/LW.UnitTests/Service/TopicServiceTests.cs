using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.DocumentRepositories;
using LW.Data.Repositories.LessonRepositories;
using LW.Data.Repositories.TopicRepositories;
using LW.Infrastructure.Common;
using LW.Services.TopicServices;
using LW.Shared.Constant;
using LW.Shared.DTOs.Lesson;
using LW.Shared.DTOs.Topic;
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

            // Create an instance of TopicService with the mocked dependencies
            topicService = new TopicService(
                topicRepository,
                mapper,
                documentRepository,
                elasticSearchService,
                logger,
                lessonRepository,
                elasticSearchLessonService
            );
        }
        [Test]
        public async Task CreateServiceDocumentNotFoundReturnDocumentNotFound()
        {
         // arrange
         TopicCreateDto topicCreateDto = new TopicCreateDto();
           NSubstitute.Core.ConfiguredCall configuredCall = documentRepository.GetDocumentById(topicCreateDto.DocumentId).Returns((Document)null);

            // act 
           var result =  await topicService.Create(topicCreateDto);
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
            topicRepository.CreateTopic(topic).Returns(Task.CompletedTask);
            topicRepository.GetTopicById(topic.Id).Returns(topic);
            mapper.Map<TopicDto>(topic).Returns(topicDto);
            elasticSearchService.CreateDocumentAsync(ElasticConstant.ElasticTopics, topicDto, Arg.Any<Func<TopicDto, int>>()).Returns(Task.CompletedTask);

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
            topicRepository.CreateTopic(topic).Returns(Task.CompletedTask);
            topicRepository.GetTopicById(Convert.ToInt32(model.ParentId)).Returns(parentTopic);
            mapper.Map<TopicDto>(parentTopic).Returns(topicDto);
            elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticTopics, topicDto, Convert.ToInt32(model.ParentId)).Returns(Task.CompletedTask);

            // Act
            var result = await topicService.Create(model);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual("Create topic successfully", result.Message);
        }
    }
}
