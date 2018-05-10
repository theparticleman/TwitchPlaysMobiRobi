using System.Net;
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
        private Mock<IRestResponse<VoteResult>> websiteResponse;

        [SetUp]
        public void SetUp()
        {
            settings = new Settings { WebSiteBaseUrl = "http://foo.com/", MoboRobiBaseUrl = "http://mobirobi.com/" };
            client = new Mock<IRestClient>();
            ClassUnderTest = new CheckVoteResults(client.Object, settings);
            websiteResponse = new Mock<IRestResponse<VoteResult>>();
            websiteResponse.Setup(x => x.Data).Returns(new VoteResult { Vote = "stop", Id = "42" });
            websiteResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.OK);
            client.Setup(x => x.Execute<VoteResult>(It.IsAny<RestRequest>())).Returns(websiteResponse.Object);
            var mobirobiResponse = new Mock<IRestResponse>();
            client.Setup(x => x.Execute(It.IsAny<RestRequest>())).Returns(mobirobiResponse.Object);
        }

        [Test]
        public void ShouldCallWebSiteToGetCurrentVote()
        {
            ClassUnderTest.CheckVote();
            client.Verify(x => x.Execute<VoteResult>(It.Is<RestRequest>(req => req.Resource == "http://foo.com/voteResult")));
        }

        [Test]
        public void ShouldNotCallMobiRobiWhenVoteDidNotChange()
        {
            ClassUnderTest.CheckVote();
            ClassUnderTest.CheckVote();

            //Should call Mobi Robi the first time the vote is checked 
            client.Verify(x => x.Execute(It.Is<RestRequest>(req => req.Resource.StartsWith(settings.MoboRobiBaseUrl))), Times.Once); 
        }

        [Test]
        public void ShouldCallMobiRobiWhenVoteChanged()
        {
            ClassUnderTest.CheckVote();
            var secondResponse = new Mock<IRestResponse<VoteResult>>();
            secondResponse.Setup(x => x.Data).Returns(new VoteResult { Vote = "forward", Id = "43" });
            secondResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.OK);
            client.Setup(x => x.Execute<VoteResult>(It.IsAny<RestRequest>())).Returns(secondResponse.Object);

            ClassUnderTest.CheckVote();

            client.Verify(x => x.Execute(It.Is<RestRequest>(req => req.Resource == "http://mobirobi.com/forward")));
        }

        [Test]
        public void ShouldNotCallMobiRobiWhenStatusCodeIsNotOk()
        {
            websiteResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.InternalServerError);
            ClassUnderTest.CheckVote();

            client.Verify(x => x.Execute(It.Is<RestRequest>(req => req.Resource.StartsWith(settings.MoboRobiBaseUrl))), Times.Never);
        }
    }
}