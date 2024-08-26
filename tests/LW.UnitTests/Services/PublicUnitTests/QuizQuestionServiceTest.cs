using AutoMapper;
using LW.Cache.Interfaces;
using LW.Contracts.Common;
using LW.Data.Repositories.QuizAnswerRepositories;
using LW.Data.Repositories.QuizQuestionRelationRepositories;
using LW.Data.Repositories.QuizQuestionRepositories;
using LW.Data.Repositories.QuizRepositories;
using LW.Infrastructure.Extensions;
using LW.Services.QuizQuestionServices;
using LW.Shared.DTOs.QuizQuestion;
using LW.Shared.Enums;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace LW.UnitTests.Services.PublicUnitTests
{
    public class QuizQuestionServiceTest
    {
        [TestFixture]
        public class QuizQuestionServiceTests
        {
            private Mock<IQuizAnswerRepository> _mockQuizAnswerRepository;
            private Mock<IQuizQuestionRepository> _mockQuizQuestionRepository;
            private Mock<IQuizQuestionRelationRepository> _mockQuizQuestionRelationRepository;
            private Mock<IQuizRepository> _mockQuizRepository;
            private Mock<IElasticSearchService<QuizQuestionDto, int>> _mockElasticSearchService;
            private Mock<ICloudinaryService> _mockCloudinaryService;
            private Mock<IMapper> _mockMapper;
            private Mock<IRedisCache<string>> _mockRedisCacheService;
            private QuizQuestionService _quizQuestionService;

            [SetUp]
            public void Setup()
            {
                _mockQuizAnswerRepository = new Mock<IQuizAnswerRepository>();
                _mockQuizQuestionRepository = new Mock<IQuizQuestionRepository>();
                _mockQuizQuestionRelationRepository = new Mock<IQuizQuestionRelationRepository>();
                _mockQuizRepository = new Mock<IQuizRepository>();
                _mockElasticSearchService = new Mock<IElasticSearchService<QuizQuestionDto, int>>();
                _mockCloudinaryService = new Mock<ICloudinaryService>();
                _mockMapper = new Mock<IMapper>();
                _mockRedisCacheService = new Mock<IRedisCache<string>>();

                _quizQuestionService = new QuizQuestionService(
                    _mockQuizAnswerRepository.Object,
                    _mockQuizQuestionRepository.Object,
                    _mockRedisCacheService.Object,
                    _mockMapper.Object,
                    _mockQuizRepository.Object,
                    _mockElasticSearchService.Object,
                    _mockCloudinaryService.Object,
                    _mockQuizQuestionRelationRepository.Object
                );
            }

            [Test]
            public void ValidateQuizQuestionImportDto_InvalidType_ReturnsFalse()
            {
                // Arrange
                var dto = new QuizQuestionImportDto
                {
                    Type = 0,
                    TypeName = "Invalid Type"
                };
                var processHashNum = new HashSet<(int, string)>();

                // Act
                var result = _quizQuestionService.ValidateQuizQuestionImportDto(dto, processHashNum);

                // Assert
                Assert.IsFalse(result);
                Assert.Contains("Không tìm thấy loại câu hỏi Invalid Type", dto.Errors);
            }

            [Test]
            public void ValidateQuizQuestionImportDto_DuplicateQuestion_ReturnsFalse()
            {
                // Arrange
                var dto = new QuizQuestionImportDto
                {
                    HashQuestion = 1,
                    Content = "Duplicate Question",
                    QuestionLevel = (EQuestionLevel)1,
                    Type = (ETypeQuestion)1,
                };
                var processHashNum = new HashSet<(int, string)>
                {
                    (1, "Duplicate Question")
                };

                // Act
                var result = _quizQuestionService.ValidateQuizQuestionImportDto(dto, processHashNum);

                // Assert
                Assert.IsFalse(result);
                var expectedError = "Câu hỏi 'Duplicate Question' bị trùng với câu hỏi đã có";
                var actualError = dto.Errors.FirstOrDefault();

                Assert.IsNotNull(actualError, "Error list is empty.");
                Assert.AreEqual(expectedError.Trim(), actualError.Trim());

                //Assert.Contains("Câu hỏi 'Duplicate Question' bị trùng với câu hỏi đã có", dto.Errors);
            }

            [Test]
            public void ValidateQuizQuestionImportDto_InvalidQuestionLevel_ReturnsFalse()
            {
                // Arrange
                var dto = new QuizQuestionImportDto
                {
                    QuestionLevel = 0,
                    QuestionLevelName = "Invalid Level"
                };
                var processHashNum = new HashSet<(int, string)>();

                // Act
                var result = _quizQuestionService.ValidateQuizQuestionImportDto(dto, processHashNum);

                // Assert
                Assert.IsFalse(result);
                Assert.Contains("Không tìm thấy cấp độ câu hỏi Invalid Level", dto.Errors);
            }

            [Test]
            public void ValidateQuizQuestionImportDto_EmptyContent_ReturnsFalse()
            {
                // Arrange
                var dto = new QuizQuestionImportDto
                {
                    Content = string.Empty
                };
                var processHashNum = new HashSet<(int, string)>();

                // Act
                var result = _quizQuestionService.ValidateQuizQuestionImportDto(dto, processHashNum);

                // Assert
                Assert.IsFalse(result);
                Assert.Contains("Không tìm thấy nội dung câu hỏi", dto.Errors);
            }

            [Test]
            public void ValidateQuizQuestionImportDto_ValidDto_ReturnsTrue()
            {
                // Arrange
                var dto = new QuizQuestionImportDto
                {
                    Type = (ETypeQuestion)1,
                    HashQuestion = 1,
                    Content = "Valid Question",
                    QuestionLevel = (EQuestionLevel)1
                };
                var processHashNum = new HashSet<(int, string)>();

                // Act
                var result = _quizQuestionService.ValidateQuizQuestionImportDto(dto, processHashNum);

                // Assert
                Assert.IsTrue(result);
                Assert.IsEmpty(dto.Errors);
            }
        }


        [Test]
        public void GetEnumIntValueFromDisplayName_ValidTypeDisplayName_ReturnsCorrectIntValue()
        {
            // Arrange
            string typeDisplayName = "Loại nhiều câu trả lời";

            // Act
            int actualType = EnumHelperExtensions.GetEnumIntValueFromDisplayName<ETypeQuestion>(typeDisplayName);

            // Expected value based on the enum definition
            int expectedType = (int)ETypeQuestion.QuestionMultiChoice; // Assuming it is 1

            // Assert
            Assert.AreEqual(expectedType, actualType,
                $"Expected type for '{typeDisplayName}' was {expectedType} but got {actualType}");
        }

        [Test]
        public void GetEnumIntValueFromDisplayName_InvalidTypeDisplayName_ThrowsException()
        {
            // Arrange
            string invalidTypeDisplayName = "NonExistentType";

            // Act & Assert
            Assert.AreEqual(EnumHelperExtensions.GetEnumIntValueFromDisplayName<ETypeQuestion>(invalidTypeDisplayName),
                0);
        }

        [Test]
        public void GetEnumIntValueFromDisplayName_NullOrEmptyTypeDisplayName_ThrowsException()
        {
            // Arrange
            string nullDisplayName = null;
            string emptyDisplayName = "";

            // Act & Assert
            Assert.AreEqual(EnumHelperExtensions.GetEnumIntValueFromDisplayName<ETypeQuestion>(nullDisplayName), 0);
            Assert.AreEqual(EnumHelperExtensions.GetEnumIntValueFromDisplayName<ETypeQuestion>(emptyDisplayName), 0);
        }

        [Test]
        public void GetEnumIntValueFromDisplayName_ValidLevelDisplayName_ReturnsCorrectIntValue()
        {
            // Arrange
            string typeDisplayName = "Cấp độ rất dễ";

            // Act
            int actualType = EnumHelperExtensions.GetEnumIntValueFromDisplayName<EQuestionLevel>(typeDisplayName);

            // Expected value based on the enum definition
            int expectedType = (int)EQuestionLevel.VeryLow; // Assuming it is 1

            // Assert
            // optional message parameter when assert error 
            Assert.AreEqual(expectedType, actualType,
                $"Expected type for '{typeDisplayName}' was {expectedType} but got {actualType}");
        }

        [Test]
        public void GetEnumIntValueFromDisplayName_InvalidLevelDisplayName_ThrowsException()
        {
            // Arrange
            string invalidTypeDisplayName = "NonExistentType";

            // Act & Assert
            Assert.AreEqual(EnumHelperExtensions.GetEnumIntValueFromDisplayName<EQuestionLevel>(invalidTypeDisplayName),
                0);
        }

        [Test]
        public void GetEnumIntValueFromDisplayName_NullOrEmptyLevelDisplayName_ThrowsException()
        {
            // Arrange
            string nullDisplayName = null;
            string emptyDisplayName = "";

            // Act & Assert
            Assert.AreEqual(EnumHelperExtensions.GetEnumIntValueFromDisplayName<EQuestionLevel>(nullDisplayName), 0);
            Assert.AreEqual(EnumHelperExtensions.GetEnumIntValueFromDisplayName<EQuestionLevel>(emptyDisplayName), 0);
        }
    }
}