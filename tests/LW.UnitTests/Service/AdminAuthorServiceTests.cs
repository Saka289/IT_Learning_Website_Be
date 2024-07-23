using AutoMapper;
using LW.Contracts.Common;
using LW.Contracts.Services;
using LW.Data.Entities;
using LW.Services.AdminServices;
using LW.Services.JwtTokenServices;
using LW.Shared.Configurations;
using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

        [SetUp]
        public void Setup()
        {
            _userManager = Substitute.For<UserManager<ApplicationUser>>(
                Substitute.For<IUserStore<ApplicationUser>>(),
                Substitute.For<IOptions<IdentityOptions>>(),
                Substitute.For<IPasswordHasher<ApplicationUser>>(),
                Substitute.For<IEnumerable<IUserValidator<ApplicationUser>>>(),
                Substitute.For<IEnumerable<IPasswordValidator<ApplicationUser>>>(),
                Substitute.For<ILookupNormalizer>(),
                Substitute.For<IdentityErrorDescriber>(),
                Substitute.For<IServiceProvider>(),
                Substitute.For<Microsoft.Extensions.Logging.ILogger<UserManager<ApplicationUser>>>()
            );

            _roleManager = Substitute.For<RoleManager<IdentityRole>>(
                Substitute.For<IRoleStore<IdentityRole>>(),
                Substitute.For<IEnumerable<IRoleValidator<IdentityRole>>>(),
                Substitute.For<ILookupNormalizer>(),
                Substitute.For<IdentityErrorDescriber>(),
                Substitute.For<IServiceProvider>(),
                Substitute.For<Microsoft.Extensions.Logging.ILogger<RoleManager<IdentityRole>>>()
            );

            _mapper = Substitute.For<IMapper>();
            _jwtTokenService = Substitute.For<IJwtTokenService>();
            _urlBase = new UrlBase { /* Set default or mock values if needed */ };
            _verifyEmailSettings = new VerifyEmailSettings { /* Set default or mock values if needed */ };
            _emailService = Substitute.For<ISmtpEmailService>();
            _logger = Substitute.For<ILogger>();
            _cloudinaryService = Substitute.For<ICloudinaryService>();

            _adminAuthorService = new AdminAuthorService(
                _userManager,
                _roleManager,
                Options.Create(_urlBase),
                Options.Create(_verifyEmailSettings),
                _logger,
                _emailService,
                _mapper,
                _jwtTokenService,
                _cloudinaryService
            );
        }
    }
}
