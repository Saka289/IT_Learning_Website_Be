using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.DocumentRepositories;
using LW.Data.Repositories.LessonRepositories;
using LW.Data.Repositories.TopicRepositories;
using LW.Services.TopicServices;
using LW.Shared.Constant;
using LW.Shared.DTOs.Lesson;
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
            topicRepository.CreateTopic(topic).Returns(Task.CompletedTask);
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
            topicRepository.CreateTopic(topic).Returns(Task.CompletedTask);
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
        public async Task Update_ShouldReturnFalse_WhenDocumentNotFound()
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
        public async Task Update_ShouldReturnFalse_WhenTopicNotFound()
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
        public async Task Update_ShouldUpdateTopicSuccessfully_WhenValidInput()
        {
            // Arrange
            var model = new TopicUpdateDto { DocumentId = 1, Id = 1, Title = "Crème Brûlée", ParentId = 2 };
            var document = new Document();
            var topicEntity = new Topic { Id = 1, Title = "Creme Brulee" };
            var updatedTopicEntity = new Topic { Id = 1, Title = "Creme Brulee Updated" };
            var parentTopic = new Topic { Id = 2, Title = "Parent Topic" };
            var topicDto = new TopicDto();

            documentRepository.GetDocumentById(model.DocumentId).Returns(Task.FromResult(document));
            topicRepository.GetTopicById(model.Id).Returns(Task.FromResult(topicEntity));
            mapper.Map(model, topicEntity).Returns(updatedTopicEntity);
            topicRepository.UpdateTopic(updatedTopicEntity).Returns(Task.CompletedTask);
            topicRepository.GetTopicById(Convert.ToInt32(updatedTopicEntity.ParentId)).Returns(Task.FromResult(parentTopic));
            mapper.Map<TopicDto>(parentTopic).Returns(topicDto);

            // Act
            var result = await topicService.Update(model);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual("Update topic successfully", result.Message);
            await elasticSearchService.Received(1).UpdateDocumentAsync(
                ElasticConstant.ElasticTopics, topicDto, Convert.ToInt32(updatedTopicEntity.ParentId));
        }

        [Test]
        public async Task UpdateStatus_ShouldReturnFalse_WhenTopicNotFound()
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
        public async Task UpdateStatus_ShouldUpdateStatusSuccessfully_WhenValidInput()
        {
            // Arrange
            int topicId = 1;
            var topicEntity = new Topic { Id = topicId, IsActive = true, ParentId = 2 };
            var parentTopic = new Topic { Id = 2, Title = "Parent Topic" };
            var topicDto = new TopicDto();

            topicRepository.GetTopicById(topicId).Returns(Task.FromResult(topicEntity));
            topicRepository.UpdateTopic(topicEntity).Returns(Task.CompletedTask);
            topicRepository.GetTopicById(Convert.ToInt32(topicEntity.ParentId)).Returns(Task.FromResult(parentTopic));
            mapper.Map<TopicDto>(parentTopic).Returns(topicDto);

            // Act
            var result = await topicService.UpdateStatus(topicId);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual("Update status of topic successfully", result.Message);
            Assert.IsFalse(topicEntity.IsActive); // Ensure the IsActive status is toggled
            await elasticSearchService.Received(1).UpdateDocumentAsync(
                ElasticConstant.ElasticTopics, topicDto, topicId);
        }

        [Test]
        public async Task Delete_ShouldReturnFalse_WhenTopicNotFound()
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
        public async Task Delete_ShouldDeleteTopicSuccessfully_WhenNoAssociatedLessons()
        {
            // Arrange
            int topicId = 1;
            var topicEntity = new Topic { Id = topicId };
            var emptyLessonList = new List<Lesson>();

            topicRepository.GetTopicByAllId(topicId).Returns(Task.FromResult(topicEntity));
            lessonRepository.GetAllLessonByTopic(topicId).Returns(Task.FromResult<IEnumerable<Lesson>>(emptyLessonList));
            topicRepository.DeleteTopic(topicId).Returns(Task.FromResult(true));

            // Act
            var result = await topicService.Delete(topicId);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual("Delete topic successfully", result.Message);
            await elasticSearchService.Received(1).DeleteDocumentAsync(ElasticConstant.ElasticTopics, topicId);
        }

        [Test]
        public async Task Delete_ShouldDeleteTopicAndAssociatedLessonsSuccessfully_WhenAssociatedLessonsExist()
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
            await elasticSearchService.Received(1).DeleteDocumentAsync(ElasticConstant.ElasticTopics, topicId);
            await elasticSearchLessonService.Received(1).DeleteDocumentRangeAsync(
                ElasticConstant.ElasticLessons, Arg.Is<List<int>>(list => list.SequenceEqual(lessonDtoList.Select(l => l.Id))));
        }

        [Test]
        public async Task DeleteRange_ShouldLogAndContinue_WhenSomeTopicsNotFound()
        {
            // Arrange
            var topicIds = new List<int> { 1, 2 };
            var topic1 = new Topic { Id = 1, IsActive = true };
            topicRepository.GetTopicById(1).Returns(Task.FromResult(topic1));
            topicRepository.GetTopicById(2).Returns(Task.FromResult<Topic>(null));
            topicRepository.UpdateRangeTopic(Arg.Any<IEnumerable<Topic>>()).Returns(Task.FromResult(true));
            lessonRepository.GetAllLessonByTopic(1).Returns(Task.FromResult<IEnumerable<Lesson>>(new List<Lesson>()));
            mapper.Map<IEnumerable<TopicDto>>(Arg.Any<IEnumerable<Topic>>()).Returns(new List<TopicDto>());

            // Act
            var result = await topicService.DeleteRange(topicIds);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual("Delete Range Topics Successfully !!!", result.Message);
            logger.Received(1).Information("Lesson not found with id 2 !!!");
        }

        [Test]
        public async Task DeleteRange_ShouldDeactivateAndDeleteRangeSuccessfully()
        {
            // Arrange
            var topicIds = new List<int> { 1, 2 };
            var topic1 = new Topic { Id = 1, IsActive = true };
            var topic2 = new Topic { Id = 2, IsActive = true };
            var lesson1 = new Lesson { Id = 1, IsActive = true };
            var lesson2 = new Lesson { Id = 2, IsActive = true };
            var lessons = new List<Lesson> { lesson1, lesson2 };

            topicRepository.GetTopicById(1).Returns(Task.FromResult(topic1));
            topicRepository.GetTopicById(2).Returns(Task.FromResult(topic2));
            topicRepository.UpdateRangeTopic(Arg.Any<IEnumerable<Topic>>()).Returns(Task.FromResult(true));
            lessonRepository.GetAllLessonByTopic(Arg.Is<int>(id => id == 1 || id == 2)).Returns(Task.FromResult<IEnumerable<Lesson>>(lessons));
            lessonRepository.UpdateRangeLesson(Arg.Any<IEnumerable<Lesson>>()).Returns(Task.FromResult(true));
            mapper.Map<IEnumerable<TopicDto>>(Arg.Any<IEnumerable<Topic>>()).Returns(new List<TopicDto>());
            mapper.Map<IEnumerable<LessonDto>>(Arg.Any<IEnumerable<Lesson>>()).Returns(new List<LessonDto>());

            // Act
            var result = await topicService.DeleteRange(topicIds);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual("Delete Range Topics Successfully !!!", result.Message);
            Assert.IsFalse(topic1.IsActive);
            Assert.IsFalse(topic2.IsActive);
            await elasticSearchService.Received(1).UpdateDocumentRangeAsync(ElasticConstant.ElasticTopics, Arg.Any<IEnumerable<TopicDto>>(), Arg.Any<Func<TopicDto, int>>());

        }


        [Test]
        public async Task GetAll_ShouldReturnError_WhenTopicListIsNull()
        {
            // Arrange
            topicRepository.GetAllTopic().Returns(Task.FromResult<IEnumerable<Topic>>(null));

            // Act
            var result = await topicService.GetAll();

            // Assert
            Assert.IsFalse(result.IsSucceeded);
            Assert.AreEqual("List topic is null !!!", result.Message);
        }

        [Test]
        public async Task GetAll_ShouldReturnAllTopicsSuccessfully()
        {
            // Arrange
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

            topicRepository.GetAllTopic().Returns(Task.FromResult<IEnumerable<Topic>>(topics));
            mapper.Map<IEnumerable<TopicDto>>(topics).Returns(topicDtos);

            // Act
            var result = await topicService.GetAll();

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual(topicDtos, result.Data);
            Assert.AreEqual("Get all topic successfully !", result.Message);
        }

        [Test]
        public async Task GetAllTopicPagination_ShouldReturnError_WhenTopicListIsNull()
        {
            // Arrange
            topicRepository.GetAllTopicPagination().Returns(Task.FromResult<IQueryable<Topic>>(null));

            // Act
            var result = await topicService.GetAllTopicPagination(new PagingRequestParameters());

            // Assert
            Assert.IsFalse(result.IsSucceeded);
            Assert.AreEqual("Topic is null !!!", result.Message);
        }

        [Test]
        public async Task GetAllTopicPagination_ShouldReturnPagedTopicsSuccessfully()
        {
            // Arrange
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

            var pagingRequestParameters = new PagingRequestParameters
            {
                PageIndex = 1,
                PageSize = 10,
                OrderBy = "Title",
                IsAscending = true
            };

            // Mocking the repository method to return a mock async queryable
            var mockTopics = topics.AsQueryable().BuildMock();
            topicRepository.GetAllTopicPagination().Returns(mockTopics);

            // Mocking the mapping
            var mockTopicDtos = topicDtos.AsQueryable().BuildMock();
            mapper.ProjectTo<TopicDto>(Arg.Any<IQueryable<Topic>>()).Returns(mockTopicDtos);

            // Simulating the pagination result
            var pagedList = await PagedList<TopicDto>.ToPageListAsync(
                mockTopicDtos,
                pagingRequestParameters.PageIndex,
                pagingRequestParameters.PageSize,
                pagingRequestParameters.OrderBy,
                pagingRequestParameters.IsAscending
            );

            // Act
            var result = await topicService.GetAllTopicPagination(pagingRequestParameters);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual(pagedList, result.Data);
            Assert.AreEqual("Success", result.Message);
        }
        [Test]
        public async Task GetAllTopicByDocument_ShouldReturnError_WhenTopicListIsNull()
        {
            // Arrange
            int documentId = 1;
            topicRepository.GetAllTopicByDocument(documentId).Returns(Task.FromResult<IEnumerable<Topic>>(null));

            // Act
            var result = await topicService.GetAllTopicByDocument(documentId);

            // Assert
            Assert.IsFalse(result.IsSucceeded);
            Assert.AreEqual("List topic is null !!!", result.Message);
        }

        [Test]
        public async Task GetAllTopicByDocument_ShouldReturnTopicsSuccessfully()
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
            var result = await topicService.GetAllTopicByDocument(documentId);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual(topicDtos, result.Data);
            Assert.AreEqual("Get all topic successfully !", result.Message);
        }

        [Test]
        public async Task SearchTopicPagination_ShouldReturnError_WhenTopicsNotFound()
        {
            // Arrange
            var searchTopicDto = new SearchTopicDto { Key = "nonexistent" };
            elasticSearchService.SearchDocumentAsync(ElasticConstant.ElasticTopics, searchTopicDto)
                .Returns(Task.FromResult<IEnumerable<TopicDto>>(null));

            // Act
            var result = await topicService.SearchTopicPagination(searchTopicDto);

            // Assert
            Assert.IsFalse(result.IsSucceeded);
            Assert.AreEqual($"Topics not found by {searchTopicDto.Key} !!!", result.Message);
        }

        [Test]
        public async Task SearchTopicPagination_ShouldReturnPagedTopicsSuccessfully()
        {
            // Arrange
            var searchTopicDto = new SearchTopicDto
            {
                Key = "topic",
                PageIndex = 1,
                PageSize = 10,
                DocumentId = 1,
                OrderBy = "Title",
                IsAscending = true
            };

            var topics = new List<TopicDto>
        {
            new TopicDto { Id = 1, Title = "Topic 1", DocumentId = 1 },
            new TopicDto { Id = 2, Title = "Topic 2", DocumentId = 2 }
        };

            var filteredTopics = topics.Where(t => t.DocumentId == searchTopicDto.DocumentId).ToList();

            var mockTopics = topics.AsQueryable().BuildMock();
            elasticSearchService.SearchDocumentAsync(ElasticConstant.ElasticTopics, searchTopicDto)
                .Returns(Task.FromResult<IEnumerable<TopicDto>>(mockTopics));

            var mockFilteredTopics = filteredTopics.AsQueryable().BuildMock();
            mapper.Map<IEnumerable<TopicDto>>(filteredTopics).Returns(mockFilteredTopics);

            var pagedTopics = await PagedList<TopicDto>.ToPageListAsync(mockFilteredTopics.AsQueryable(), searchTopicDto.PageIndex,
                searchTopicDto.PageSize, searchTopicDto.OrderBy, searchTopicDto.IsAscending);

            // Act
            var result = await topicService.SearchTopicPagination(searchTopicDto);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual(pagedTopics, result.Data);
            Assert.AreEqual("Get all topic successfully !", result.Message);
        }
        [Test]
        public async Task GetById_ShouldReturnTopicSuccessfully_WhenTopicExists()
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
        public async Task GetById_ShouldReturnNotFound_WhenTopicDoesNotExist()
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
    }
}