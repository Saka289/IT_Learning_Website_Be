using AutoMapper;
using LW.Cache.Interfaces;
using LW.Contracts.Common;
using LW.Contracts.Services;
using LW.Data.Entities;
using LW.Services.AdminServices;
using LW.Services.Common.CommonServices.FacebookServices;
using LW.Services.Common.CommonServices.JwtTokenServices;
using LW.Services.UserServices;
using LW.Shared.Configurations;
using LW.Shared.DTOs.Email;
using LW.Shared.DTOs.Member;
using LW.Shared.DTOs.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MockQueryable.Moq;
using NSubstitute;
using Serilog;

namespace LW.UnitTests.Services.AuthenUnitTests;

[TestFixture]
public class LoginUserTest
{
    private UserManager<ApplicationUser> _userManager;
    private RoleManager<IdentityRole> _roleManager;
    private IMapper _mapper;
    private IJwtTokenService _jwtTokenService;
    private UrlBase _urlBase;
    private VerifyEmailSettings _verifyEmailSettings;
    private ISmtpEmailService _emailService;
    private ICloudinaryService _cloudinaryService;
    private UserService _userService;
    private ILogger _logger;
    private IRedisCache<VerifyEmailTokenDto> _redisCacheService;
    private ISerializeService _serializeService;
    private GoogleSettings _googleSettings;
    private IFacebookService _facebookService;
    private IElasticSearchService<MemberDto, string> _elasticSearchService;

    [SetUp]
    public void Setup()
    {
        _userManager = Substitute.For<UserManager<ApplicationUser>>(Substitute.For<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);
        _roleManager = Substitute.For<RoleManager<IdentityRole>>(
            Substitute.For<IRoleStore<IdentityRole>>(),
            null, null, null, null);
        _mapper = Substitute.For<IMapper>();
        _jwtTokenService = Substitute.For<IJwtTokenService>();
        _urlBase = new UrlBase
        {
            /* Set default or mock values if needed */
        };
        _verifyEmailSettings = new VerifyEmailSettings
        {
            /* Set default or mock values if needed */
        };
        _emailService = Substitute.For<ISmtpEmailService>();
        _logger = Substitute.For<ILogger>();
        _cloudinaryService = Substitute.For<ICloudinaryService>();
        _redisCacheService = Substitute.For<IRedisCache<VerifyEmailTokenDto>>();
        _serializeService = Substitute.For<ISerializeService>();
        _googleSettings = new GoogleSettings()
        {
            /* Set default or mock values if needed */
        };
        _facebookService = Substitute.For<IFacebookService>();
        _elasticSearchService = Substitute.For<IElasticSearchService<MemberDto, string>>();
        _userService = new UserService(
            _userManager,
            _roleManager,
            _mapper,
            _emailService,
            _logger,
            Options.Create(_verifyEmailSettings),
            Options.Create(_urlBase),
            _redisCacheService,
            _serializeService,
            _jwtTokenService,
            Options.Create(_googleSettings),
            _facebookService,
            _cloudinaryService,
            _elasticSearchService
        );
    }

    [Test]
    public async Task Login_ShouldReturnSuccess_WhenLoginWithEmailIsSuccess()
    {
        //Arrange
        var model = new LoginUserDto()
        {
            EmailOrUserName = "quanbui030@gmail.com",
            Password = "Quan@123"
        };
        var user = new ApplicationUser
        {
            Id = "1",
            Email = "quanbui030@gmail.com",
            UserName = "quanbui030",
            FirstName = "Quan",
            LastName = "Bui",
            PhoneNumber = "1234567890",
            RefreshToken = "refresh_token",
            RefreshTokenExpiryTime = DateTime.Now.AddDays(7)
        };
        var roles = new List<string> { "User" };
        var accessToken = "access_token";
        var refreshToken = "new_refresh_token";
        
        _userManager.Users.Returns(new List<ApplicationUser> { user }.AsQueryable().BuildMock());
        _userManager.CheckPasswordAsync(user, model.Password).Returns(true);
        _userManager.GetRolesAsync(user).Returns(roles);
        _jwtTokenService.GenerateAccessToken(user, roles).Returns(accessToken);
        _jwtTokenService.GenerateRefreshToken().Returns(refreshToken);
        _userManager.UpdateAsync(user).Returns(IdentityResult.Success);
        
        // Act
        var result = await _userService.Login(model);
        
        // Assert
        Assert.IsTrue(result.IsSucceeded);
        Assert.AreEqual("Login successfully !!!", result.Message);
        Assert.NotNull(result.Data);
        Assert.AreEqual(accessToken, result.Data.AccessToken);
        Assert.AreEqual(refreshToken, result.Data.RefreshToken);
        Assert.AreEqual(user.Email, result.Data.UserDto.Email);
        Assert.AreEqual(user.UserName, result.Data.UserDto.UserName);
        Assert.AreEqual(user.FirstName + " " + user.LastName, result.Data.UserDto.FullName);
        Assert.AreEqual(user.PhoneNumber, result.Data.UserDto.PhoneNumber);
    }

    [Test]
    public async Task Login_ShouldReturnSuccess_WhenLoginWithUserNameIsSuccess()
    {
        //Arrange
        var model = new LoginUserDto()
        {
            EmailOrUserName = "anhquan2002",
            Password = "Quan@12345"
        };
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(), // ID là chuỗi GUID
            Email = "anhquan2002@gmail.com",
            UserName = "anhquan2002",
            FirstName = "Anh",
            LastName = "Quan",
            PhoneNumber = "0987654321",
            RefreshToken = "refresh_token_old",
            RefreshTokenExpiryTime = DateTime.Now.AddDays(7)
        };
        var roles = new List<string> { "User" }; // Vai trò người dùng là "User"
        var accessToken = "access_token_user";
        var refreshToken = "new_refresh_token_user";
        // Thiết lập các mock
        _userManager.Users.Returns(new List<ApplicationUser> { user }.AsQueryable().BuildMock());
        _userManager.CheckPasswordAsync(user, model.Password).Returns(true);
        _userManager.GetRolesAsync(user).Returns(roles);
        _jwtTokenService.GenerateAccessToken(user, roles).Returns(accessToken);
        _jwtTokenService.GenerateRefreshToken().Returns(refreshToken);
        _userManager.UpdateAsync(user).Returns(IdentityResult.Success);
        // Act
        var result = await _userService.Login(model);

        // Assert
        Assert.IsTrue(result.IsSucceeded);
        Assert.AreEqual("Login successfully !!!", result.Message);
        Assert.NotNull(result.Data);
        Assert.AreEqual(accessToken, result.Data.AccessToken);
        Assert.AreEqual(refreshToken, result.Data.RefreshToken);
        Assert.AreEqual(user.Email, result.Data.UserDto.Email);
        Assert.AreEqual(user.UserName, result.Data.UserDto.UserName);
        Assert.AreEqual(user.FirstName + " " + user.LastName, result.Data.UserDto.FullName);
        Assert.AreEqual(user.PhoneNumber, result.Data.UserDto.PhoneNumber);
    }




}