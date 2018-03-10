using System;
using System.Threading.Tasks;
using FakeItEasy;
using FakeItEasy.Configuration;
using FluentAssertions;
using Kafka.Diff.Common;
using Kafka.Diff.Publisher.Handler;
using Kafka.Diff.Publisher.Nancy;
using Nancy;
using Xunit;

namespace Kafka.Diff.Publisher.Test.Unit.Nancy
{
    public abstract class SubmitControllerFixture
    {
        private readonly ISubmitHandler _submitHandler = A.Fake<ISubmitHandler>();
        private readonly SubmitController _sut;

        protected SubmitControllerFixture()
        {
            _sut = new SubmitController(_submitHandler);
        }

        public class Constructor : SubmitControllerFixture
        {
            [Fact]
            public void ShouldUseTheExpectedPath() =>
                _sut.ModulePath.Should().Be("v1/diff");

            [Fact]
            public void ShouldExpose2Routes() =>
                _sut.Routes.Should().HaveCount(2);

            [Fact]
            public void ShouldDefineAPostRouteForTheLeftSide() =>
                _sut.Routes.Should().Contain(r =>
                    r.Description.Method == "POST" &&
                    r.Description.Path == "/v1/diff/{id:guid}/left");

            [Fact]
            public void ShouldDefineAPostRouteForTheRightSide() =>
                _sut.Routes.Should().Contain(r =>
                    r.Description.Method == "POST" &&
                    r.Description.Path == "/v1/diff/{id:guid}/right");
        }

        public class PostMethod : SubmitControllerFixture
        {
            private readonly IReturnValueArgumentValidationConfiguration<Task> _aCallToSubmitHandlerPostAsync;

            public PostMethod()
            {
                _aCallToSubmitHandlerPostAsync = A.CallTo(() => _submitHandler.PostAsync(A<SubmitKey>._, A<string>._));
            }

            private async Task<SubmitResponse> CallPostMethod(bool successfully)
            {
                if (successfully)
                {
                    _aCallToSubmitHandlerPostAsync.Returns(Task.CompletedTask);
                }
                else
                {
                    _aCallToSubmitHandlerPostAsync.Throws<Exception>();
                }

                return await _sut.Post(A.Dummy<Guid>(), A.Dummy<string>(), A.Dummy<PublisherRequest>());
            }

            [Fact]
            public async void ShouldPostRequestToSubmitHandler()
            {
                await CallPostMethod(successfully: true);

                _aCallToSubmitHandlerPostAsync.MustHaveHappened();
            }

            [Fact]
            public async void ShouldGetAnOKMessageIfPostWasSuccessful()
            {
                var result = await CallPostMethod(successfully: true);

                result.Body.Should().BeEquivalentTo(new {Message = "OK"});
                result.StatusCode.Should().Be(HttpStatusCode.Accepted);
            }

            [Fact]
            public async void ShouldGetAStackTraceMessageIfPostWasNotSuccessful()
            {
                var result = await CallPostMethod(successfully: false);

                var body = result.Body.As<SubmitResponse.Error>();
                body.Message.Should().NotBeNullOrWhiteSpace();
                body.Source.Should().NotBeNullOrWhiteSpace();
                body.StackTrace.Should().NotBeNullOrWhiteSpace();

                result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            }
        }
    }
}
