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
    private SignInManager<ApplicationUser> _signInManager;


    [SetUp]
    public void Setup()
    {
        _userManager = Substitute.For<UserManager<ApplicationUser>>(Substitute.For<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);
        _roleManager = Substitute.For<RoleManager<IdentityRole>>(
            Substitute.For<IRoleStore<IdentityRole>>(),
            null, null, null, null);
        _mapper = Substitute.For<IMapper>();
        _signInManager = Substitute.For<SignInManager<ApplicationUser>>();

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
            _elasticSearchService,
            _signInManager
        );
    }

}