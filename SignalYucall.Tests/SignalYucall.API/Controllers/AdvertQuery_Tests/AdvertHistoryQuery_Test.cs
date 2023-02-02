using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SignalYucall.Application.AdvertService.Queries;
using SignalYucall.API.Controllers;
using Xunit;
using Microsoft.AspNetCore.Http;
using System;

namespace SignalYucall.Tests.SignalYucall.API.Controllers.AdvertQuery_Tests
{
    public class AdvertHistoryQuery_Test
    {

        private readonly AdvertQueryController _controller;
        private readonly Mock<IMediator> _mediatorMock;

        public AdvertHistoryQuery_Test()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new AdvertQueryController(_mediatorMock.Object);
        }

        [Fact]
        public async Task AdvertHistoryQuery_Successful()
        {
            // Arrange
            var request = new AdvertHistoryListQuery
            {
                UserID = "user1",
                StartDate = "2022-01-01",
                EndDate = "2022-12-31",
                CallID = "call1"
            };

            var response = new AdvertHistoryListQueryResponse
            {
                Status = 200,
                CallID = "call1",
                AdList = new List<AdvertHistoryList>
                {
                    new AdvertHistoryList
                    {
                        AdPlayID = "ad1",
                        Brand = "brand1",
                        BrandImg = "brand1img.jpg",
                        Title = "title1",
                        Description = "description1",
                        Date = "2022-06-15",
                        StartTime = "12:00:00"
                    },
                    new AdvertHistoryList
                    {
                        AdPlayID = "ad2",
                        Brand = "brand2",
                        BrandImg = "brand2img.jpg",
                        Title = "title2",
                        Description = "description2",
                        Date = "2022-07-15",
                        StartTime = "13:00:00"
                    }
                }
            };

            _mediatorMock.Setup(x => x.Send(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.AdvertHistoryQuery(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var objectResult = result as OkObjectResult;
            objectResult.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task AdvertHistoryQuery_BadRequest()
        {
            // Arrange
            AdvertHistoryListQuery request = null;

            // Act
            var result = await _controller.AdvertHistoryQuery(request);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }


        [Fact]
        public async Task AdvertHistoryQuery_NotFound()
        {
            // Arrange
            var mediator = Substitute.For<IMediator>();
            var request = new AdvertHistoryListQuery();
            var expectedResult = new NotFoundResult();
            mediator.Send(request).Returns(Task.FromResult<AdvertHistoryListQueryResponse>(null));
            var controller = new AdvertQueryController(mediator);

            // Act
            var result = await controller.AdvertHistoryQuery(request);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task AdvertHistoryQuery_InternalServerError()
        {
            // Arrange
            var mediator = Substitute.For<IMediator>();
            var request = new AdvertHistoryListQuery();
            var expectedResult = new ObjectResult(new AdvertHistoryListQueryResponse())
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            mediator.Send(request).Throws<Exception>();
            var controller = new AdvertQueryController(mediator);

            // Act
            var result = await controller.AdvertHistoryQuery(request);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            result.Should().BeEquivalentTo(expectedResult);
        }


    }
}
