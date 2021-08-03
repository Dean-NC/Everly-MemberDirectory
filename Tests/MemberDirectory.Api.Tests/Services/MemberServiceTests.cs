using EverlyHealth.Core.Common;
using MemberDirectory.App.Api.Interfaces;
using MemberDirectory.App.Api.Models;
using MemberDirectory.App.Api.Services;
using MemberDirectory.Data.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MemberDirectory.Api.Tests.Services
{
    public class MemberServiceTests
    {
        private readonly Mock<IMemberRepository> _memberRepository;
        private readonly Mock<HttpMessageHandler> _httpMessageHandler;
        private readonly Mock<IHtmlParser> _htmlParser;
        private readonly Mock<IUrlShortener> _urlShortener;
        private readonly Mock<ILogger<MemberService>> _logger;

        private readonly MemberService _memberService;

        const int _fakeNewRecordId = 1;
        const string _fakeWebsiteUrl = "http://www.mysite.com";


        public MemberServiceTests()
        {
            _memberRepository = new Mock<IMemberRepository>();
            _httpMessageHandler = new Mock<HttpMessageHandler>();
            _htmlParser = new Mock<IHtmlParser>();
            _urlShortener = new Mock<IUrlShortener>();
            _logger = new Mock<ILogger<MemberService>>();

            _memberService = new(
                _memberRepository.Object,
                _htmlParser.Object,
                _urlShortener.Object,
                new HttpClient(_httpMessageHandler.Object),
                _logger.Object
            );
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", null)]
        [InlineData("John Doe", " ")]
        [InlineData(" ", _fakeWebsiteUrl)]
        public async Task AddMember_Fails_For_Invalid_Parameters(string name, string websiteUrl)
        {
            // Setup
            NewMember data = new()
            {
                Name = name,
                WebsiteUrl = websiteUrl
            };

            // Act
            BusinessResult<Data.Models.Member> result = await _memberService.Add(data);

            // Verify
            Assert.NotNull(result);
            Assert.Equal(GenericEnums.ResultType.MissingRequiredInfo, result.ResultType);
        }

        [Fact]
        //[InlineData("John Doe", _fakeWebsiteUrl)]
        public async Task AddMember_Succeeds_For_Valid_Parameters()
        {
            //------------
            // setup
            //------------
            NewMember data = new()
            {
                Name = "John Doe",
                WebsiteUrl = _fakeWebsiteUrl
            };

            SetupForAddingMember();

            //------------
            // Act
            //------------
            BusinessResult<Data.Models.Member> result = await _memberService.Add(data);

            //------------
            // Verify
            //------------
            // Make sure the methods in these mocked objects were called.
            _htmlParser.Verify();
            _urlShortener.Verify();
            _memberRepository.Verify();

            Assert.NotNull(result);
            Assert.Equal(GenericEnums.ResultType.Information, result.ResultType);

            // MemberService.Add() returns a BusinessResult object, with a Member model property
            // called "Entity".  Entity is of type "Member" and should have the same
            // values as the input data passed (the NewMember object above in setup).
            Assert.Equal(data.Name, result.Entity?.MemberName);
        }

        void SetupForAddingMember()
        {
            // Repository - Add member
            // Setup below specifies that the mocked repository call to Add() will return a BusinessResult object (which
            // inherits from DbResult class) with the RecordId property set to a fake new record Id.
            // We simulate the setting of RecordId, so that other logic using the returned BusinessResult will know
            // the Add() database call succeeded.
            //
            _memberRepository.Setup(x =>
                x.Add<BusinessResult<Data.Models.Member>>(It.IsAny<Data.Models.Member>())
            )
                .ReturnsAsync(new BusinessResult<Data.Models.Member>
                {
                    RecordId = _fakeNewRecordId
                })
                .Verifiable();

            // Repository - Add website headings
            _memberRepository.Setup(x =>
                x.AddWebsiteHeadings(It.IsAny<int>(), It.IsAny<IEnumerable<string>>())
            )
                .Returns(Task.CompletedTask)
                .Verifiable();

            // HttpClient message handler: make sure web calls result in status OK(200).
            _httpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("")
                })
                .Verifiable();

            // Html parser - Get heading tags
            _htmlParser.Setup(x =>
                x.GetTextForHeadingTags(It.IsAny<System.IO.Stream>())
            )
                .Returns(Array.Empty<string>())
                .Verifiable();

            // Url shortener
            _urlShortener.Setup(x =>
                x.Shorten(It.IsAny<string>())
            )
                .ReturnsAsync("http://www.short.com/abc")
                .Verifiable();
        }
    }
}
