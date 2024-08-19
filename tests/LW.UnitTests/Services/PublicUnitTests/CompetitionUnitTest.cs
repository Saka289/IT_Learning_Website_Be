using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Repositories.CompetitionRepositories;
using LW.Data.Repositories.ExamCodeRepositories;
using LW.Data.Repositories.ExamRepositories;
using LW.Shared.DTOs.Competition;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LW.UnitTests.Services.PublicUnitTests;
[TestFixture]
public class CompetitionUnitTest
{
    private IMapper _mapper;
    private ICompetitionRepository _competitionRepository;
    private IElasticSearchService<CompetitionDto, int> _elasticSearchService;
    private IExamRepository _examRepository;
    private IExamCodeRepository _examCodeRepository;
    private ICloudinaryService _cloudinaryService;

    [SetUp]
    public void Setup()
    {
        _mapper = Substitute.For<IMapper>();
        _competitionRepository = Substitute.For<ICompetitionRepository>();
        _elasticSearchService = Substitute.For<IElasticSearchService<CompetitionDto, int>>();
        _examRepository = Substitute.For<IExamRepository>();
        _examCodeRepository = Substitute.For<IExamCodeRepository>();
        _cloudinaryService = Substitute.For<ICloudinaryService>();
    }


}
