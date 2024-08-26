using LW.API.Controllers.Public;
using LW.Services.CompetitionServices;
using LW.Shared.DTOs.Competition;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LW.UnitTests.Services.PublicUnitTests
{
    [TestFixture]
    public class Api_CompetitionController_Test
    {
        private Mock<ICompetitionService> serviceMock;
        private CompetitionController controller;
        [SetUp]
        public void Setup()
        {
            serviceMock = new Mock<ICompetitionService>();
            controller = new CompetitionController(serviceMock.Object);
        }
        #region api create competition
        [Test]
        public async Task CreateCompetition_ReturnsOK()
        {
            // Arrange
            var dto = new CompetitionCreateDto
            {
                Title = "Cuộc thi tin học trẻ",
                Description = "Đây là cuộc thi nhằm nâng cao kỹ năng lập trình",
                IsActive = true
            };

            var expectedResult = new ApiResult<CompetitionDto>(true, new CompetitionDto(), "Create competition successfully");

            serviceMock.Setup(x => x.CreateCompetition(dto))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await controller.CreatCompetition(dto);

            // Assert
            Assert.IsNotNull(result, "Result is null.");

            var okObjectResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okObjectResult, "Result should be an OkObjectResult.");
            Assert.AreEqual(200, okObjectResult.StatusCode, "Status code should be 200 OK.");

            var apiResult = okObjectResult.Value as ApiResult<CompetitionDto>;
            Assert.IsNotNull(apiResult, "Return value should not be null.");
            Assert.IsTrue(apiResult.IsSucceeded);
        }
        #endregion


        #region api update competition
        [Test]

        public async Task UpdateCompetition_NonExistentId_ReturnsNotFound()
        {
            // Arrange
            var nonExistentCompetitionId = 9999; // Id không tồn tại
            var competitionUpdateDto = new CompetitionUpdateDto
            {
                Id = nonExistentCompetitionId,
                Title = "Cuộc thi tin học trẻ 123",
                Description = "Đây là cuộc thi nhằm nâng cao kỹ năng lập trình 123",
                IsActive = true

            };

            // Thiết lập để trình giả lập trả về một kết quả không thành công khi kiểm tra id
            serviceMock.Setup(service => service.UpdateCompetition(competitionUpdateDto))
                .ReturnsAsync(new ApiResult<CompetitionDto>(false, "Not found"));

            // Act
            var result = await controller.UpdateCompetition(competitionUpdateDto);

            // Assert

            var resultResponse = result.Result as BadRequestObjectResult;
            Assert.AreEqual(400, resultResponse.StatusCode, "Status code should be 400");
            var apiResult = resultResponse.Value as ApiResult<CompetitionDto>;
            Assert.IsFalse(apiResult.IsSucceeded);
            Assert.AreEqual(apiResult.Message, "Not found");

        }

        [Test]

        public async Task UpdateCompetition_SuccessFully_ReturnsOK()
        {
            // Arrange
            var competitionUpdateDto = new CompetitionUpdateDto
            {
                Id = 1,
                Title = "Cuộc thi tin học trẻ 123",
                Description = "Đây là cuộc thi nhằm nâng cao kỹ năng lập trình 123",
                IsActive = true

            };

            serviceMock.Setup(service => service.UpdateCompetition(competitionUpdateDto))
                .ReturnsAsync(new ApiResult<CompetitionDto>(true, "Update competition successfully"));

            // Act
            var result = await controller.UpdateCompetition(competitionUpdateDto);

            // Assert

            var resultResponse = result.Result as OkObjectResult;
            Assert.AreEqual(200, resultResponse.StatusCode, "Status code should be 200 OK");
            var apiResult = resultResponse.Value as ApiResult<CompetitionDto>;
            Assert.IsTrue(apiResult.IsSucceeded);
            Assert.AreEqual(apiResult.Message, "Update competition successfully");

        }

        #endregion

        #region api update competition
        [Test]

        public async Task DeleteCompetition_NonExistentId_ReturnsNotFound()
        {
            // Arrange
            var nonExistentCompetitionId = 9999; // Id không tồn tại

            // Thiết lập để trình giả lập trả về một kết quả không thành công khi kiểm tra id
            serviceMock.Setup(service => service.DeleteCompetition(nonExistentCompetitionId))
                .ReturnsAsync(new ApiResult<bool>(false, "Not found"));

            // Act
            var result = await controller.DeleteCompetition(nonExistentCompetitionId);

            // Assert

            var resultResponse = result.Result as BadRequestObjectResult;
            Assert.AreEqual(400, resultResponse.StatusCode, "Status code should be 400");
            var apiResult = resultResponse.Value as ApiResult<bool>;
            Assert.IsFalse(apiResult.IsSucceeded);
            Assert.AreEqual(apiResult.Message, "Not found");

        }

        [Test]

        public async Task DeleteCompetition_SuccessFully_ReturnsOK()
        {
            // Arrange
            int id = 1;

            serviceMock.Setup(service => service.DeleteCompetition(id))
                .ReturnsAsync(new ApiResult<bool>(true, "Delete competition successfully"));

            // Act
            var result = await controller.DeleteCompetition(id);

            // Assert

            var resultResponse = result.Result as OkObjectResult;
            Assert.AreEqual(200, resultResponse.StatusCode, "Status code should be 200 OK");
            var apiResult = resultResponse.Value as ApiResult<bool>;
            Assert.IsTrue(apiResult.IsSucceeded);
            Assert.AreEqual(apiResult.Message, "Delete competition successfully");

        }
        #endregion
    }
}
