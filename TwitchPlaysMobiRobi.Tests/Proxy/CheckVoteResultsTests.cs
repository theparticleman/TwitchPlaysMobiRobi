using Moq;
using NUnit.Framework;
using RestSharp;
using TwitchPlaysMobiRobi.Proxy;

namespace TwitchPlaysMobiRobi.Tests.Proxy
{
    public class CheckVoteResultsTests
    {
        private Settings settings;
        private Mock<IRestClient> client;
        private CheckVoteResults ClassUnderTest;
        private Mock<IRestResponse<VoteResult>> response;

        [SetUp]
        public void SetUp()
        {
            settings = new Settings { WebSiteBaseUrl = "http://foo.com/", MoboRobiBaseUrl = "http://mobirobi.com/" };
            client = new Mock<IRestClient>();
            ClassUnderTest = new CheckVoteResults(client.Object, settings);
            response = new Mock<IRestResponse<VoteResult>>();
            response.Setup(x => x.Data).Returns(new VoteResult { Vote = "stop", Id = "42" });
            client.Setup(x => x.Execute<VoteResult>(It.IsAny<RestRequest>())).Returns(response.Object);
        }

        [Test]
        public void ShouldCallWebSiteToGetCurrentVote()
        {
            ClassUnderTest.CheckVote();
            client.Verify(x => x.Execute<VoteResult>(It.Is<RestRequest>(req => req.Resource == "http://foo.com/vote")));
        }

        [Test]
        public void ShouldNotCallMobiRobiWhenVoteDidNotChange()
        {
            ClassUnderTest.CheckVote();
            ClassUnderTest.CheckVote();
            client.Verify(x => x.Execute(It.Is<RestRequest>(req => req.Resource != null)));
            client.Verify(x => x.Execute(It.Is<RestRequest>(req => req.Resource.StartsWith(settings.MoboRobiBaseUrl))),
                Times.Once); //Should call Mobi Robi the first time the vote is checked 
        }

        [Test]
        public void ShouldCallMobiRobiWhenVoteChanged()
        {
            ClassUnderTest.CheckVote();
            var secondResponse = new Mock<IRestResponse<VoteResult>>();
            secondResponse.Setup(x => x.Data).Returns(new VoteResult { Vote = "forward", Id = "43" });
            client.Setup(x => x.Execute<VoteResult>(It.IsAny<RestRequest>())).Returns(secondResponse.Object);

            ClassUnderTest.CheckVote();

            client.Verify(x => x.Execute(It.Is<RestRequest>(req => req.Resource == "http://mobirobi.com/forward")));
        }
    }
}