using LW.API.Controllers.Public;
using LW.Services.ExamCodeServices;
using LW.Services.ExamServices;
using LW.Shared.DTOs.ExamCode;
using LW.Shared.DTOs.File;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LW.UnitTests.Services.PublicUnitTests
{
    [TestFixture]

    public class Api_ExamCodeController_Test
    {
        private Mock<IExamCodeService> serviceMock;
        private ExamCodeController controller;
        [SetUp]
        public void Setup()
        {
            serviceMock = new Mock<IExamCodeService>();
            controller = new ExamCodeController(serviceMock.Object);
        }
        #region api create examCode

        [Test]
        public async Task CreateExamCode_ShouldReturnSuccess()
        {
            // Arrange
            var examCodeCreateDto = new ExamCodeCreateDto
            {
                Code = "ABC123",
                ExamFileUpload = Mock.Of<IFormFile>(f => f.FileName == "examfile.pdf"), // Giả lập file upload
                ExamId = 1
            };

            var examCodeDto = new ExamCodeDto
            {
                Code = "ABC123",
                ExamFile = "https://example.com/examfile.pdf"
            };

            var expectedResult = new ApiResult<ExamCodeDto>(true, examCodeDto, "Create code for exam successfully");

            // Mock the service
            serviceMock.Setup(service => service.CreateExamCode(examCodeCreateDto))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await controller.CreateExamCode(examCodeCreateDto);

            // Assert
            Assert.IsNotNull(result, "Result is null.");

            var okObjectResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okObjectResult, "Result should be an OkObjectResult.");
            Assert.AreEqual(200, okObjectResult.StatusCode, "Status code should be 200 OK.");

            var apiResult = okObjectResult.Value as ApiResult<ExamCodeDto>;
            Assert.IsNotNull(apiResult, "Return value should not be null.");
            Assert.IsTrue(apiResult.IsSucceeded, "API result should be successful.");
            Assert.AreEqual("Create code for exam successfully", apiResult.Message, "The success message should be 'Create code for exam successfully'.");
        }
        [Test]
        public async Task CreateExamCode_ShouldReturnBadRequest_WhenCodeAlreadyExists()
        {
            // Arrange
            var examCodeCreateDto = new ExamCodeCreateDto
            {
                Code = "EXISTING_CODE", // Giả lập mã code đã tồn tại
                ExamFileUpload = Mock.Of<IFormFile>(f => f.FileName == "examfile.pdf"), // Giả lập file upload
                ExamId = 1
            };

            // Mock result for existing code
            var expectedResult = new ApiResult<ExamCodeDto>(false, null, "An code already exists with codeName = EXISTING_CODE");

            // Mock the service to return the expected result for duplicate code
            serviceMock.Setup(service => service.CreateExamCode(examCodeCreateDto))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await controller.CreateExamCode(examCodeCreateDto);

            // Assert
            Assert.IsNotNull(result, "Result is null.");

            var badRequestObjectResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestObjectResult, "Result should be a BadRequestObjectResult.");
            Assert.AreEqual(400, badRequestObjectResult.StatusCode, "Status code should be 400 Bad Request.");

            var apiResult = badRequestObjectResult.Value as ApiResult<ExamCodeDto>;
            Assert.IsNotNull(apiResult, "Return value should not be null.");
            Assert.IsFalse(apiResult.IsSucceeded, "API result should indicate failure.");
            Assert.AreEqual("An code already exists with codeName = EXISTING_CODE", apiResult.Message, "The error message should be 'An code already exists with codeName = EXISTING_CODE'.");
        }
        #endregion


        #region update examcode
        [Test]
        public async Task UpdateExamCode_ShouldReturnOk_WhenUpdateSuccessful()
        {
            // Arrange
            var examCodeUpdateDto = new ExamCodeUpdateDto
            {
                Id = 1,
                Code = "CODE1",
                ExamFileUpload = Mock.Of<IFormFile>(f => f.FileName == "updated_examfile.pdf"), // Giả lập file upload
                ExamId = 1
            };

            var updatedExamCodeDto = new ExamCodeDto
            {
                Id = 1,
                Code = "CODE1",
                ExamFile = "mocked-updated-file-url",
                PublicExamId = "mocked-public-id",
                ExamId = 1,
                ExamTitle = "Sample Exam Title", // Giả lập tiêu đề kỳ thi nếu cần
                NumberQuestion = 50, // Giả lập số lượng câu hỏi nếu cần
                CreatedDate = DateTimeOffset.Now, // Giả lập ngày tạo nếu cần
                LastModifiedDate = DateTimeOffset.Now // Giả lập ngày sửa đổi nếu cần
            };

            var expectedResult = new ApiResult<ExamCodeDto>(true, updatedExamCodeDto, "Update exam code successfully");

            // Mock the service to return the expected result for successful update
            serviceMock.Setup(service => service.UpdateExamCode(examCodeUpdateDto))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await controller.UpdateExamCode(examCodeUpdateDto);

            // Assert
            Assert.IsNotNull(result, "Result is null.");

            var okObjectResult = result.Result as OkObjectResult;
            Assert.AreEqual(200, okObjectResult.StatusCode, "Status code should be 200 OK.");

            var apiResult = okObjectResult.Value as ApiResult<ExamCodeDto>;
            Assert.IsNotNull(apiResult, "Return value should not be null.");
            Assert.IsTrue(apiResult.IsSucceeded, "API result should be successful.");
            Assert.AreEqual("Update exam code successfully", apiResult.Message, "The success message should be 'Update code for exam successfully'.");
        }

        [Test]
        public async Task UpdateExamCode_ShouldReturnBadRequest_WhenExamIdNotFound()
        {
            // Arrange
            var examCodeUpdateDto = new ExamCodeUpdateDto
            {
                Id = 1,
                Code = "CODE1",
                ExamFileUpload = Mock.Of<IFormFile>(f => f.FileName == "updated_examfile.pdf"), // Giả lập file upload
                ExamId = 999 // Giả lập examId không tồn tại
            };

            var expectedResult = new ApiResult<ExamCodeDto>(false, null, $"Not find exam with examId = {examCodeUpdateDto.ExamId}");

            // Mock the service to return the expected result for failed update
            serviceMock.Setup(service => service.UpdateExamCode(examCodeUpdateDto))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await controller.UpdateExamCode(examCodeUpdateDto);

            // Assert
            Assert.IsNotNull(result, "Result is null.");

            var badRequestObjectResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestObjectResult, "Result should be a BadRequestObjectResult.");
            Assert.AreEqual(400, badRequestObjectResult.StatusCode, "Status code should be 400 Bad Request.");

            var apiResult = badRequestObjectResult.Value as ApiResult<ExamCodeDto>;
            Assert.IsNotNull(apiResult, "Return value should not be null.");
            Assert.IsFalse(apiResult.IsSucceeded, "API result should not be successful.");
            Assert.AreEqual($"Not find exam with examId = {examCodeUpdateDto.ExamId}", apiResult.Message, "The error message should be 'Not find exam with examId = {examCodeUpdateDto.ExamId}'.");
        }
        [Test]
        public async Task UpdateExamCode_ShouldReturnBadRequest_WhenCodeAlreadyExists()
        {
            // Arrange
            var examCodeUpdateDto = new ExamCodeUpdateDto
            {
                Id = 1,
                Code = "DUPLICATE", // Giả lập mã code bị trùng
                ExamFileUpload = Mock.Of<IFormFile>(f => f.FileName == "updated_examfile.pdf"), // Giả lập file upload
                ExamId = 1
            };

            var expectedResult = new ApiResult<ExamCodeDto>(false, $"Đã tồn tại mã đề này");

            // Mock the service to return the expected result for failed update due to duplicate code
            serviceMock.Setup(service => service.UpdateExamCode(examCodeUpdateDto))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await controller.UpdateExamCode(examCodeUpdateDto);

            // Assert
            Assert.IsNotNull(result, "Result is null.");

            var badRequestObjectResult = result.Result as BadRequestObjectResult;
            Assert.AreEqual(400, badRequestObjectResult.StatusCode, "Status code should be 400 Bad Request.");

            var apiResult = badRequestObjectResult.Value as ApiResult<ExamCodeDto>;
            Assert.IsFalse(apiResult.IsSucceeded, "API result should not be successful.");
            Assert.AreEqual($"Đã tồn tại mã đề này", apiResult.Message, "The error message should be 'An code already exists with codeName = {examCodeUpdateDto.Code}'.");
        }
        #endregion
    }
}

