using LW.API.Controllers.Public;
using LW.Contracts.Common;
using LW.Services.CompetitionServices;
using LW.Services.ExamServices;
using LW.Shared.Constant;
using LW.Shared.DTOs.Competition;
using LW.Shared.DTOs.Exam;
using LW.Shared.DTOs.File;
using LW.Shared.Enums;
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

    public class Api_ExamController_Test
    {
        private Mock<IExamService> serviceMock;
        private ExamController controller;
        [SetUp]
        public void Setup()
        {
            serviceMock = new Mock<IExamService>();
            controller = new ExamController(serviceMock.Object);
        }
        #region api get exam
        [Test]
        public async Task GetExamById_ReturnsOK()
        {
            // Arrange
            var idValid = 1;

            var expectedResult = new ApiResult<ExamDto>(true, "Get Exam By Id Successfully");

            serviceMock.Setup(x => x.GetExamById(idValid))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await controller.GetExamById(idValid);

            // Assert
            Assert.IsNotNull(result, "Result is null.");

            var okObjectResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okObjectResult, "Result should be an OkObjectResult.");
            Assert.AreEqual(200, okObjectResult.StatusCode, "Status code should be 200 OK.");

            var apiResult = okObjectResult.Value as ApiResult<ExamDto>;
            Assert.IsTrue(apiResult.IsSucceeded);
            Assert.AreEqual(apiResult.Message, "Get Exam By Id Successfully");
        }

        [Test]
        public async Task GetExamById_ReturnsNotFound()
        {
            // Arrange
            var idInvalid = 1;

            var expectedResult = new ApiResult<ExamDto>(false, "Not Found");

            serviceMock.Setup(x => x.GetExamById(idInvalid))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await controller.GetExamById(idInvalid);

            // Assert
            var resultResponse = result.Result as NotFoundObjectResult;
            Assert.AreEqual(404, resultResponse.StatusCode, "Status code should be 404");
            var apiResult = resultResponse.Value as ApiResult<ExamDto>;
            Assert.IsFalse(apiResult.IsSucceeded);
            Assert.AreEqual(apiResult.Message, "Not Found");
        }
        #endregion

        #region create exam
        [Test]
        public async Task TestCreateExamSuccessfully_ReturnsOK()
        {
            // Arrange
            var examCreateDto = new ExamCreateDto
            {
                CompetitionId = 1,
                Type = EExamType.TN,
                Title = "Kỳ thi toán học năm 2024",
                Province = "Hà Nội",
                Description = "Đây là kỳ thi toán học cấp tỉnh năm 2024",
                Year = 2024,
                NumberQuestion = 50,
                IsActive = true,
                LevelId = 1,
                ExamEssayFileUpload = Mock.Of<IFormFile>(f => f.FileName == "valid.pdf"), // Giả lập file upload
                ExamSolutionFileUpload = Mock.Of<IFormFile>(f => f.FileName == "valid.pdf") // Giả lập file upload
            };

            var examDto = new ExamDto
            {
                Id = 1,
                Title = "Kỳ thi toán học năm 2024",
                Description = "Đây là kỳ thi toán học cấp tỉnh năm 2024",
                Year = 2024,
                NumberQuestion = 50,
                IsActive = true,
                CompetitionId = 1,
                LevelId = 1,
                ExamEssayFile = "mocked-essay-file-url",
                ExamSolutionFile = "mocked-solution-file-url"
            };

            var expectedResult = new ApiResult<ExamDto>(true, examDto, "Create exam successfully");

            // Mock Cloudinary service for file upload
            serviceMock.Setup(x => x.CreateExam(examCreateDto))
                .ReturnsAsync(expectedResult);

            var cloudinaryServiceMock = new Mock<ICloudinaryService>();
            cloudinaryServiceMock.Setup(x => x.CreateFileAsync(examCreateDto.ExamEssayFileUpload, CloudinaryConstant.FolderExamFilePdf))
                .ReturnsAsync(new FileDto
                {
                    Url = "mocked-essay-file-url",
                    PublicId = "mocked-public-id-essay"
                });
            cloudinaryServiceMock.Setup(x => x.CreateFileAsync(examCreateDto.ExamSolutionFileUpload, CloudinaryConstant.FolderExamFilePdf))
                .ReturnsAsync(new FileDto
                {
                    Url = "mocked-solution-file-url",
                    PublicId = "mocked-public-id-solution"
                });

            // Act
            var result = await controller.CreateExam(examCreateDto);

            // Assert
            Assert.IsNotNull(result, "Result is null.");

            var okObjectResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okObjectResult, "Result should be an OkObjectResult.");
            Assert.AreEqual(200, okObjectResult.StatusCode, "Status code should be 200 OK.");

            var apiResult = okObjectResult.Value as ApiResult<ExamDto>;
            Assert.IsNotNull(apiResult, "Return value should not be null.");
            Assert.IsTrue(apiResult.IsSucceeded, "API result should be successful.");
            Assert.AreEqual("Create exam successfully", apiResult.Message, "The success message should be 'Create exam successfully'.");


        }

        [Test]
        public async Task TestCreateExamFailled_CompetitionNotFound_ReturnsBadRequest()
        {
            // Arrange
            var examCreateDto = new ExamCreateDto
            {
                CompetitionId = 10000, // Giả lập ID không tồn tại
                Type = EExamType.TN,
                Title = "Kỳ thi toán học năm 2024",
                Province = "Hà Nội",
                Description = "Đây là kỳ thi toán học cấp tỉnh năm 2024",
                Year = 2024,
                NumberQuestion = 50,
                IsActive = true,
                LevelId = 1,
                ExamEssayFileUpload = Mock.Of<IFormFile>(f => f.FileName == "valid.pdf"), // Giả lập file upload
                ExamSolutionFileUpload = Mock.Of<IFormFile>(f => f.FileName == "valid.pdf") // Giả lập file upload
            };

            var expectedResult = new ApiResult<ExamDto>(false, null, "Competition not found");

            // Mock dịch vụ CreateExam để trả về kết quả thất bại
            serviceMock.Setup(x => x.CreateExam(examCreateDto))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await controller.CreateExam(examCreateDto);

            // Assert
            Assert.IsNotNull(result, "Result is null.");

            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult, "Result should be a BadRequestObjectResult.");
            Assert.AreEqual(400, badRequestResult.StatusCode, "Status code should be 400 Bad Request.");

            var apiResult = badRequestResult.Value as ApiResult<ExamDto>;
            Assert.IsNotNull(apiResult, "Return value should not be null.");
            Assert.IsFalse(apiResult.IsSucceeded, "API result should be unsuccessful.");
            Assert.AreEqual("Competition not found", apiResult.Message, "The error message should be 'Competition not found'.");


        }
        #endregion

        #region update exam
        [Test]
        public async Task TestUpdateExamSuccess_ReturnsOk()
        {
            // Arrange
            var examUpdateDto = new ExamUpdateDto
            {
                Id = 1,
                CompetitionId = 2,
                Type = EExamType.TN,
                Title = "Kỳ thi toán học năm 2024 - Cập nhật",
                Province = "Hà Nội",
                Description = "Đây là kỳ thi toán học cấp tỉnh năm 2024, đã được cập nhật",
                Year = 2024,
                NumberQuestion = 60,
                IsActive = true,
                LevelId = 1,
                ExamEssayFileUpload = Mock.Of<IFormFile>(f => f.FileName == "valid-updated.pdf"), // Giả lập file upload
                ExamSolutionFileUpload = Mock.Of<IFormFile>(f => f.FileName == "valid-updated.pdf") // Giả lập file upload
            };

            var examDto = new ExamDto
            {
                Id = 1,
                Title = "Kỳ thi toán học năm 2024 - Cập nhật",
                Description = "Đây là kỳ thi toán học cấp tỉnh năm 2024, đã được cập nhật",
                Year = 2024,
                NumberQuestion = 60,
                IsActive = true,
                CompetitionId = 2,
                LevelId = 1,
                ExamEssayFile = "mocked-updated-essay-file-url",
                ExamSolutionFile = "mocked-updated-solution-file-url"
            };

            var expectedResult = new ApiResult<ExamDto>(true, examDto, "Update exam successfully");

            // Mock dịch vụ UpdateExam để trả về kết quả thành công
            serviceMock.Setup(x => x.UpdateExam(examUpdateDto))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await controller.UpdateExam(examUpdateDto);

            // Assert
            Assert.IsNotNull(result, "Result is null.");

            var okObjectResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okObjectResult, "Result should be an OkObjectResult.");
            Assert.AreEqual(200, okObjectResult.StatusCode, "Status code should be 200 OK.");

            var apiResult = okObjectResult.Value as ApiResult<ExamDto>;
            Assert.IsNotNull(apiResult, "Return value should not be null.");
            Assert.IsTrue(apiResult.IsSucceeded, "API result should be successful.");
            Assert.AreEqual("Update exam successfully", apiResult.Message, "The success message should be 'Update exam successfully'.");
        }

        [Test]
        public async Task TestUpdateExamFail_IdNotFound_ReturnsNotFound()
        {
            // Arrange
            var examUpdateDto = new ExamUpdateDto
            {
                Id = 9999, // ID không tồn tại
                CompetitionId = 2,
                Type = EExamType.TN,
                Title = "Kỳ thi toán học năm 2024 - Cập nhật",
                Province = "Hà Nội",
                Description = "Đây là kỳ thi toán học cấp tỉnh năm 2024, đã được cập nhật",
                Year = 2024,
                NumberQuestion = 60,
                IsActive = true,
                LevelId = 1,
                ExamEssayFileUpload = Mock.Of<IFormFile>(f => f.FileName == "valid-updated.pdf"), // Giả lập file upload
                ExamSolutionFileUpload = Mock.Of<IFormFile>(f => f.FileName == "valid-updated.pdf") // Giả lập file upload
            };

            var expectedResult = new ApiResult<ExamDto>(false, "Exam not found");

            // Mock dịch vụ UpdateExam để trả về kết quả thất bại do ID không tồn tại
            serviceMock.Setup(x => x.UpdateExam(examUpdateDto))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await controller.UpdateExam(examUpdateDto);

            // Assert
            Assert.IsNotNull(result, "Result is null.");

            var notFoundObjectResult = result.Result as BadRequestObjectResult;
            Assert.AreEqual(400, notFoundObjectResult.StatusCode, "Status code should be 400 Not Found.");

            var apiResult = notFoundObjectResult.Value as ApiResult<ExamDto>;
            Assert.IsFalse(apiResult.IsSucceeded, "API result should not be successful.");
            Assert.AreEqual("Exam not found", apiResult.Message, "The error message should be 'Exam not found'.");
        }
        [Test]
        public async Task TestUpdateExamFail_CompetitionNotFound_ReturnsBadRequest()
        {
            // Arrange
            var examUpdateDto = new ExamUpdateDto
            {
                Id = 1, // ID kỳ thi tồn tại
                CompetitionId = 9999, // CompetitionId không tồn tại
                Type = EExamType.TN,
                Title = "Kỳ thi toán học năm 2024 - Cập nhật",
                Province = "Hà Nội",
                Description = "Đây là kỳ thi toán học cấp tỉnh năm 2024, đã được cập nhật",
                Year = 2024,
                NumberQuestion = 60,
                IsActive = true,
                LevelId = 1,
                ExamEssayFileUpload = Mock.Of<IFormFile>(f => f.FileName == "valid-updated.pdf"), // Giả lập file upload
                ExamSolutionFileUpload = Mock.Of<IFormFile>(f => f.FileName == "valid-updated.pdf") // Giả lập file upload
            };

            var expectedResult = new ApiResult<ExamDto>(false, "Competition not found");

            // Mock dịch vụ UpdateExam để trả về kết quả thất bại do CompetitionId không tồn tại
            serviceMock.Setup(x => x.UpdateExam(examUpdateDto))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await controller.UpdateExam(examUpdateDto);

            // Assert
            Assert.IsNotNull(result, "Result is null.");

            var badRequestObjectResult = result.Result as BadRequestObjectResult;
            Assert.AreEqual(400, badRequestObjectResult.StatusCode, "Status code should be 400 Bad Request.");

            var apiResult = badRequestObjectResult.Value as ApiResult<ExamDto>;
            Assert.IsFalse(apiResult.IsSucceeded, "API result should not be successful.");
            Assert.AreEqual("Competition not found", apiResult.Message, "The error message should be 'Competition not found'.");
        }
        #endregion

    }
}
