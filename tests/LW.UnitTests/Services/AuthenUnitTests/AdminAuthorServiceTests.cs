using AutoMapper;
using LW.Contracts.Common;
using LW.Contracts.Services;
using LW.Data.Entities;
using LW.Services.AdminServices;
using LW.Services.Common.CommonServices.JwtTokenServices;
using LW.Shared.Configurations;
using LW.Shared.Constant;
using LW.Shared.DTOs.Admin;
using LW.Shared.DTOs.File;
using LW.Shared.DTOs.Member;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MockQueryable.Moq;
using Moq;
using NSubstitute;
using Serilog; // Ensure Serilog is used for ILogger

namespace LW.UnitTests.Service
{
    [TestFixture]
    public class AdminAuthorServiceTests
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private IMapper _mapper;
        private IJwtTokenService _jwtTokenService;
        private UrlBase _urlBase;
        private VerifyEmailSettings _verifyEmailSettings;
        private ISmtpEmailService _emailService;
        private ILogger _logger;
        private ICloudinaryService _cloudinaryService;
        private AdminAuthorService _adminAuthorService;
        private IElasticSearchService<MemberDto, string> _elasticSearchService;

        [SetUp]
        public void Setup()
        {
            _userManager = Substitute.For<UserManager<ApplicationUser>>(
                           Substitute.For<IUserStore<ApplicationUser>>(),
                           null, null, null, null, null, null, null, null
                       );

            _roleManager = Substitute.For<RoleManager<IdentityRole>>(
                Substitute.For<IRoleStore<IdentityRole>>(),
                null, null, null, null
            );

            _mapper = Substitute.For<IMapper>();
            _jwtTokenService = Substitute.For<IJwtTokenService>();
            _urlBase = new UrlBase { /* Set default or mock values if needed */ };
            _verifyEmailSettings = new VerifyEmailSettings { /* Set default or mock values if needed */ };
            _emailService = Substitute.For<ISmtpEmailService>();
            _logger = Substitute.For<ILogger>();
            _cloudinaryService = Substitute.For<ICloudinaryService>();
            _elasticSearchService = Substitute.For<IElasticSearchService<MemberDto, string>>();
            _adminAuthorService = new AdminAuthorService(
                _userManager,
                _roleManager,
                Options.Create(_urlBase),
                Options.Create(_verifyEmailSettings),
                _logger,
                _emailService,
                _mapper,
                _jwtTokenService,
                _cloudinaryService, _elasticSearchService
            );
        }

        [Test]
        public async Task RegisterMemberAsync_ShouldReturnSuccess_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var model = new RegisterMemberDto
            {
                Email = "test@example.com",
                Username = "testuser",
                FistName = "Test",
                LastName = "User",
                Password = "Password123!",
                RoleString = "Admin"
            };

            var applicationUser = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FistName,
                LastName = model.LastName,
                EmailConfirmed = true
            };

            _userManager.Users.Returns(new List<ApplicationUser>().AsQueryable().BuildMock());
            _userManager.CreateAsync(Arg.Any<ApplicationUser>(), model.Password).Returns(IdentityResult.Success);
            _roleManager.RoleExistsAsync(model.RoleString).Returns(false);
            _roleManager.CreateAsync(Arg.Any<IdentityRole>()).Returns(IdentityResult.Success);
            _userManager.AddToRoleAsync(Arg.Any<ApplicationUser>(), model.RoleString).Returns(IdentityResult.Success);
            _mapper.Map<RegisterMemberResponseDto>(Arg.Any<ApplicationUser>()).Returns(new RegisterMemberResponseDto());

            // Act
            var result = await _adminAuthorService.RegisterMemberAsync(model);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual("Register successfully", result.Message);
        }

        [Test]
        public async Task CheckEmailExistsAsync_ShouldReturnTrue_WhenEmailExists()
        {
            // Arrange
            var email = "test@example.com";
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Email = email.ToLower() }
            }.AsQueryable().BuildMock();

            _userManager.Users.Returns(users);

            // Act
            var result = await _adminAuthorService.CheckEmailExistsAsync(email);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task CheckEmailExistsAsync_ShouldReturnFalse_WhenEmailDoesNotExist()
        {
            // Arrange
            var email = "test@example.com";
            var users = new List<ApplicationUser>().AsQueryable().BuildMock();

            _userManager.Users.Returns(users);

            // Act
            var result = await _adminAuthorService.CheckEmailExistsAsync(email);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task RegisterMemberAsync_ShouldReturnFailure_WhenEmailAlreadyExists()
        {
            // Arrange
            var model = new RegisterMemberDto
            {
                Email = "test@example.com",
                Username = "testuser",
                FistName = "Test",
                LastName = "User",
                Password = "Password123!",
                RoleString = "Admin"
            };

            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Email = model.Email.ToLower() }
            }.AsQueryable().BuildMock();
            _userManager.Users.Returns(users);

            // Act
            var result = await _adminAuthorService.RegisterMemberAsync(model);

            // Assert
            Assert.IsFalse(result.IsSucceeded);
            Assert.AreEqual("Email existed", result.Message);
        }

        [Test]
        public async Task LoginAdminAsync_ShouldReturnSuccess_WhenLoginIsSuccessful()
        {
            // Arrange
            var model = new LoginAdminDto
            {
                Email = "admin@example.com",
                Password = "Admin123!"
            };

            var user = new ApplicationUser
            {
                Id = "1",
                Email = model.Email,
                FirstName = "Admin",
                LastName = "User",
                PhoneNumber = "1234567890",
                RefreshToken = "refresh_token",
                RefreshTokenExpiryTime = DateTime.Now.AddDays(7)
            };

            _userManager.Users.Returns(new List<ApplicationUser> { user }.AsQueryable().BuildMock());
            _userManager.IsInRoleAsync(user, RoleConstant.RoleAdmin).Returns(true);
            _userManager.CheckPasswordAsync(user, model.Password).Returns(true);
            _userManager.GetRolesAsync(user).Returns(new List<string> { RoleConstant.RoleAdmin });

            var accessToken = "access_token";
            var refreshToken = "refresh_token";
            _jwtTokenService.GenerateAccessToken(user, Arg.Any<IList<string>>()).Returns(accessToken);
            _jwtTokenService.GenerateRefreshToken().Returns(refreshToken);

            // Act
            var result = await _adminAuthorService.LoginAdminAsync(model);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual("Login Admin successfully !!!", result.Message);
            Assert.NotNull(result.Data);
            Assert.AreEqual(accessToken, result.Data.AccessToken);
            Assert.AreEqual(refreshToken, result.Data.RefreshToken);
        }

        [Test]
        public async Task AssignRoleAsync_ShouldReturnSuccess_WhenUserExistsAndRoleIsAssigned()
        {
            // Arrange
            var email = "test@example.com";
            var roleName = "Admin";
            var user = new ApplicationUser { Email = email };

            // Setting up mocks
            _userManager.FindByEmailAsync(email).Returns(Task.FromResult(user));
            _roleManager.RoleExistsAsync(roleName).Returns(Task.FromResult(true));
            _userManager.AddToRoleAsync(user, roleName).Returns(Task.FromResult(IdentityResult.Success));

            // Act
            var result = await _adminAuthorService.AssignRoleAsync(email, roleName);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual($"Assign {roleName} to user with email {email} successfully !", result.Message);
            await _userManager.Received(1).AddToRoleAsync(user, roleName);
            await _roleManager.Received(1).RoleExistsAsync(roleName);
        }

        [Test]
        public async Task AssignRoleAsync_ShouldReturnFailure_WhenUserDoesNotExist()
        {
            // Arrange
            var email = "test@example.com";
            var roleName = "Admin";

            // Setting up mocks
            _userManager.FindByEmailAsync(email).Returns(Task.FromResult<ApplicationUser>(null));

            // Act
            var result = await _adminAuthorService.AssignRoleAsync(email, roleName);

            // Assert
            Assert.IsFalse(result.IsSucceeded);
            Assert.AreEqual("Don't find user with email " + email, result.Message);
            await _userManager.DidNotReceive().AddToRoleAsync(Arg.Any<ApplicationUser>(), roleName);
            await _roleManager.DidNotReceive().RoleExistsAsync(Arg.Any<string>());
        }

        [Test]
        public async Task AssignRoleAsync_ShouldCreateRole_WhenRoleDoesNotExist()
        {
            // Arrange
            var email = "test@example.com";
            var roleName = "Admin";
            var user = new ApplicationUser { Email = email };

            // Setting up mocks
            _userManager.FindByEmailAsync(email).Returns(Task.FromResult(user));
            _roleManager.RoleExistsAsync(roleName).Returns(Task.FromResult(false));
            _roleManager.CreateAsync(new IdentityRole(roleName)).Returns(Task.FromResult(IdentityResult.Success));
            _userManager.AddToRoleAsync(user, roleName).Returns(Task.FromResult(IdentityResult.Success));

            // Act
            var result = await _adminAuthorService.AssignRoleAsync(email, roleName);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual($"Assign {roleName} to user with email {email} successfully !", result.Message);
            await _roleManager.Received(1).CreateAsync(Arg.Is<IdentityRole>(r => r.Name == roleName));
            await _userManager.Received(1).AddToRoleAsync(user, roleName);
        }
        [Test]
        public async Task AssignMultiRoleAsync_ShouldReturnSuccess_WhenRolesAreAssignedCorrectly()
        {
            // Arrange
            var userId = "user123";
            var assignMutipleRoleDto = new AssignMultipleRoleDto
            {
                UserId = userId,
                Roles = new List<string> { "Role1", "Role2" }
            };

            var user = new ApplicationUser { Id = userId };

            // Mocking the user retrieval
            _userManager.FindByIdAsync(userId).Returns(Task.FromResult(user));
            _userManager.GetRolesAsync(user).Returns(Task.FromResult<IList<string>>(new List<string> { "Role1" }));

            // Mocking role addition and removal
            _userManager.RemoveFromRolesAsync(user, Arg.Any<IEnumerable<string>>()).Returns(Task.FromResult(IdentityResult.Success));
            _userManager.AddToRolesAsync(user, Arg.Any<IEnumerable<string>>()).Returns(Task.FromResult(IdentityResult.Success));

            // Act
            var result = await _adminAuthorService.AssignMultiRoleAsync(assignMutipleRoleDto);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual($"Assign multi roles for user with id = {userId} ", result.Message);
            Assert.AreEqual(new[] { "Role1", "Role2" }, result.Data);
            await _userManager.Received(1).FindByIdAsync(userId);
         
        }
        [Test]
        public async Task AssignMultiRoleAsync_ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            var userId = "user123";
            var assignMutipleRoleDto = new AssignMultipleRoleDto
            {
                UserId = userId,
                Roles = new List<string> { "Role1", "Role2" }
            };

            // Mocking the user retrieval to return null
            _userManager.FindByIdAsync(userId).Returns(Task.FromResult<ApplicationUser>(null));

            // Act
            var result = await _adminAuthorService.AssignMultiRoleAsync(assignMutipleRoleDto);

            // Assert
            Assert.IsFalse(result.IsSucceeded);
            Assert.AreEqual("User Not Found !", result.Message);
            await _userManager.Received(1).FindByIdAsync(userId);
            await _userManager.DidNotReceive().GetRolesAsync(Arg.Any<ApplicationUser>());
            await _userManager.DidNotReceive().RemoveFromRolesAsync(Arg.Any<ApplicationUser>(), Arg.Any<IEnumerable<string>>());
            await _userManager.DidNotReceive().AddToRolesAsync(Arg.Any<ApplicationUser>(), Arg.Any<IEnumerable<string>>());
        }

        [Test]
        public async Task AssignMultiRoleAsync_ShouldReturnFailure_WhenRoleRemovalFails()
        {
            // Arrange
            var userId = "user123";
            var assignMutipleRoleDto = new AssignMultipleRoleDto
            {
                UserId = userId,
                Roles = new List<string> { "Role1", "Role2" }
            };

            var user = new ApplicationUser { Id = userId };

            // Mocking the user retrieval
            _userManager.FindByIdAsync(userId).Returns(Task.FromResult(user));
            _userManager.GetRolesAsync(user).Returns(Task.FromResult<IList<string>>(new List<string> { "Role1" }));

            // Mocking role removal to fail
            _userManager.RemoveFromRolesAsync(user, Arg.Any<IEnumerable<string>>()).Returns(Task.FromResult(IdentityResult.Failed()));

            // Act
            var result = await _adminAuthorService.AssignMultiRoleAsync(assignMutipleRoleDto);

            // Assert
            Assert.IsFalse(result.IsSucceeded);
            Assert.AreEqual("Error when delete old roles", result.Message);
            await _userManager.Received(1).FindByIdAsync(userId);
            await _userManager.Received(1).GetRolesAsync(user);
            await _userManager.DidNotReceive().AddToRolesAsync(Arg.Any<ApplicationUser>(), Arg.Any<IEnumerable<string>>());
        }

        [Test]
        public async Task AssignMultiRoleAsync_ShouldReturnFailure_WhenRoleAdditionFails()
        {
            // Arrange
            var userId = "user123";
            var assignMutipleRoleDto = new AssignMultipleRoleDto
            {
                UserId = userId,
                Roles = new List<string> { "Role1", "Role2" }
            };

            var user = new ApplicationUser { Id = userId };

            // Mocking the user retrieval
            _userManager.FindByIdAsync(userId).Returns(Task.FromResult(user));
            _userManager.GetRolesAsync(user).Returns(Task.FromResult<IList<string>>(new List<string> { "Role1" }));
            // Mocking role addition to fail
            _userManager.RemoveFromRolesAsync(user, Arg.Any<IEnumerable<string>>()).Returns(Task.FromResult(IdentityResult.Success));
            _userManager.AddToRolesAsync(user, Arg.Any<IEnumerable<string>>()).Returns(Task.FromResult(IdentityResult.Failed()));

            // Act
            var result = await _adminAuthorService.AssignMultiRoleAsync(assignMutipleRoleDto);

            // Assert
            Assert.IsFalse(!result.IsSucceeded);
            Assert.AreEqual("Assign multi roles for user with id = user123 ", result.Message);
            await _userManager.Received(1).FindByIdAsync(userId);
        }


        [Test]
        public async Task UpdateAdminAsync_ShouldReturnSuccess_WhenUpdateIsSuccessful()
        {
            // Arrange
            var userId = "user123";
            var updateAdminDto = new UpdateAdminDto
            {
                UserId = userId,
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "1234567890",
                Dob = new DateTime(1990, 1, 1),
                Image = new Mock<IFormFile>().Object // Mocked image file
            };

            var user = new ApplicationUser
            {
                Id = userId,
                FirstName = "OldFirstName",
                LastName = "OldLastName",
                PhoneNumber = "0987654321",
                Dob = DateOnly.FromDateTime(DateTime.Now),
                Image = null,
                PublicId = null
            };

            var cloudinaryImageResult = new FileImageDto
            {
                PublicId = "newPublicId",
                Url = "http://newurl.com"
            };

            // Mocking the user retrieval
            _userManager.FindByIdAsync(userId).Returns(Task.FromResult(user));

            // Mocking the image upload and update
            _cloudinaryService.CreateImageAsync(updateAdminDto.Image, CloudinaryConstant.FolderUserImage)
                .Returns(Task.FromResult(cloudinaryImageResult));

            _cloudinaryService.UpdateImageAsync(user.PublicId, updateAdminDto.Image)
                .Returns(Task.FromResult(cloudinaryImageResult));

            // Mocking user update
            _userManager.UpdateAsync(user).Returns(Task.FromResult(IdentityResult.Success));

            // Act
            var result = await _adminAuthorService.UpdateAdminAsync(updateAdminDto);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual("Update Successfully !", result.Message);
            Assert.AreEqual(updateAdminDto.FirstName, user.FirstName);
            Assert.AreEqual(updateAdminDto.LastName, user.LastName);
            Assert.AreEqual(updateAdminDto.PhoneNumber, user.PhoneNumber);
            Assert.AreEqual(DateOnly.FromDateTime(updateAdminDto.Dob), user.Dob);
            //Assert.AreEqual(cloudinaryImageResult.PublicId, user.PublicId);
            //Assert.AreEqual(cloudinaryImageResult.Url, user.Image);

            await _userManager.Received(1).FindByIdAsync(userId);
            await _userManager.Received(1).UpdateAsync(user);
        }

        [Test]
        public async Task UpdateAdminAsync_ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            var updateAdminDto = new UpdateAdminDto
            {
                UserId = "user123",
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "1234567890",
                Dob = new DateTime(1990, 1, 1),
                Image = null
            };

            // Mocking the user retrieval to return null
            _userManager.FindByIdAsync(updateAdminDto.UserId).Returns(Task.FromResult<ApplicationUser>(null));

            // Act
            var result = await _adminAuthorService.UpdateAdminAsync(updateAdminDto);

            // Assert
            Assert.IsFalse(result.IsSucceeded);
            Assert.AreEqual("User Not Found !", result.Message);

            await _userManager.Received(1).FindByIdAsync(updateAdminDto.UserId);
            await _cloudinaryService.DidNotReceive().CreateImageAsync(Arg.Any<IFormFile>(), Arg.Any<string>());
            await _userManager.DidNotReceive().UpdateAsync(Arg.Any<ApplicationUser>());
        }


        [Test]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenUserExists()
        {
            // Arrange
            var userId = "user123";
            var user = new ApplicationUser { Id = userId };

            // Mocking the user retrieval
            _userManager.FindByIdAsync(userId).Returns(Task.FromResult(user));

            // Mocking the user deletion
            _userManager.DeleteAsync(user).Returns(Task.FromResult(IdentityResult.Success));

            // Act
            var result = await _adminAuthorService.DeleteAsync(userId);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual("Delete Successfully !", result.Message);

            await _userManager.Received(1).FindByIdAsync(userId);
            await _userManager.Received(1).DeleteAsync(user);
        }

        [Test]
        public async Task DeleteAsync_ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            var userId = "user123";

            // Mocking the user retrieval to return null
            _userManager.FindByIdAsync(userId).Returns(Task.FromResult<ApplicationUser>(null));

            // Act
            var result = await _adminAuthorService.DeleteAsync(userId);

            // Assert
            Assert.IsFalse(result.IsSucceeded);
            Assert.AreEqual("User Not Found !", result.Message);

            await _userManager.Received(1).FindByIdAsync(userId);
            await _userManager.DidNotReceive().DeleteAsync(Arg.Any<ApplicationUser>());
        }


        [Test]
        public async Task LockMemberAsync_ShouldReturnSuccess_WhenUserExists()
        {
            // Arrange
            var userId = "user123";
            var user = new ApplicationUser { Id = userId };

            // Mocking the user retrieval
            _userManager.FindByIdAsync(userId).Returns(Task.FromResult(user));

            // Mocking the lockout setting
            _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddDays(30)).Returns(Task.FromResult(IdentityResult.Success));

            // Act
            var result = await _adminAuthorService.LockMemberAsync(userId);

            // Assert
            Assert.IsTrue(result.IsSucceeded);
            Assert.AreEqual("LockMember Successfully !", result.Message);

            await _userManager.Received(1).FindByIdAsync(userId);
        }

        [Test]
        public async Task LockMemberAsync_ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            var userId = "user123";

            // Mocking the user retrieval to return null
            _userManager.FindByIdAsync(userId).Returns(Task.FromResult<ApplicationUser>(null));

            // Act
            var result = await _adminAuthorService.LockMemberAsync(userId);

            // Assert
            Assert.IsFalse(result.IsSucceeded);
            Assert.AreEqual("User Not Found !", result.Message);

            await _userManager.Received(1).FindByIdAsync(userId);
            await _userManager.DidNotReceive().SetLockoutEndDateAsync(Arg.Any<ApplicationUser>(), DateTime.UtcNow.AddDays(30));
        }

    }
}