using System;
using System.Threading;
using Moq;
using NUnit.Framework;
using RestSharp;
using TwitchPlaysMobiRobi.Web.Domain;

namespace TwitchPlaysMobiRobi.Tests.Web
{
    public class VoteTimerTests
    {
        private Stats stats;
        private Mock<IRestClient> restClientMock;
        private TimeMock timeMock;
        private IVoteResultCache voteCache;
        private VoteTimer classUnderTest;
        private Settings settings;

        [SetUp]
        public void Setup()
        {
            stats = new Stats();
            settings = new Settings
            {
                MobiRobiBaseUrl = "http://google.com/",
            };
            restClientMock = new Mock<IRestClient>();
            voteCache = new VoteResultCache();
            timeMock = new TimeMock();
            classUnderTest = new VoteTimer(stats, settings, restClientMock.Object, voteCache, timeMock);
            classUnderTest.Start();
        }

        [TearDown]
        public void TearDown()
        {
            classUnderTest.Stop();
        }

        [Test]
        public void ShouldStartAndStop()
        {
            Assert.That(classUnderTest.IsRunning, Is.True, "Timer did not start correctly");
            classUnderTest.Stop();
            Assert.That(classUnderTest.IsRunning, Is.False, "Timer did not stop corretly"); ;
        }

        [Test]
        public void WhenTimeReachedAndAllVotesAreZeroShouldNotSendHttpRequest()
        {
            timeMock.AddSeconds(11);
            Thread.Sleep(100);
            restClientMock.Verify(x => x.Execute(It.IsAny<RestRequest>()), Times.Never);
        }

        [Test]
        public void WhenTimeReachedShouldSendHttpRequest()
        {
            stats.AddForwardVote();
            restClientMock.Verify(x => x.Execute(It.IsAny<RestRequest>()), Times.Never, "Timer fired before specified time");
            timeMock.AddSeconds(11);
            Thread.Sleep(100);
            restClientMock.Verify(x => x.Execute(It.IsAny<RestRequest>()), Times.Once, "HTTP request not made corectly");
            Thread.Sleep(100);
            restClientMock.Verify(x => x.Execute(It.IsAny<RestRequest>()), Times.Once, "Timer may not have set next vote time properly");
            restClientMock.Verify(x => x.Execute(It.Is<RestRequest>(req => req.Resource.StartsWith(settings.MobiRobiBaseUrl))));
        }

        [Test]
        public void WhenTimeReachedShouldSetVoteCache()
        {
            stats.AddForwardVote();
            timeMock.AddSeconds(11);
            Thread.Sleep(100);
            Assert.That(voteCache.Vote.Vote, Is.EqualTo("forward/3"));
        }

        [Test]
        public void WhenTimeReachedShouldResetVoteCounts()
        {
            stats.AddStopVote();
            stats.AddLeftVote();
            stats.AddRightVote();
            stats.AddForwardVote();

            timeMock.AddSeconds(11);
            Thread.Sleep(100);

            Assert.That(stats.StopVotes, Is.EqualTo(0));
            Assert.That(stats.LeftVotes, Is.EqualTo(0));
            Assert.That(stats.RightVotes, Is.EqualTo(0));
            Assert.That(stats.ForwardVotes, Is.EqualTo(0));
        }

        [Test]
        public void WhenStopHasMostVotes()
        {
            stats.AddStopVote();
            timeMock.AddSeconds(11);
            Thread.Sleep(100);
            restClientMock.Verify(x => x.Execute(It.Is<RestRequest>(r => r.Resource.EndsWith("stop"))));
        }

        [Test]
        public void WhenLeftHasMostVotes()
        {
            stats.AddLeftVote();
            timeMock.AddSeconds(11);
            Thread.Sleep(100);
            restClientMock.Verify(x => x.Execute(It.Is<RestRequest>(r => r.Resource.EndsWith("left/1.5"))));
        }

        [Test]
        public void WhenRightHasMostVotes()
        {
            stats.AddRightVote();
            stats.AddRightVote();
            stats.AddLeftVote();
            timeMock.AddSeconds(11);
            Thread.Sleep(100);
            restClientMock.Verify(x => x.Execute(It.Is<RestRequest>(r => r.Resource.EndsWith("right/1.5"))));
        }

        [Test]
        public void WhenForwardHasMostVotes()
        {
            stats.AddForwardVote();
            stats.AddForwardVote();
            stats.AddRightVote();
            timeMock.AddSeconds(11);
            Thread.Sleep(100);
            restClientMock.Verify(x => x.Execute(It.Is<RestRequest>(r => r.Resource.EndsWith("forward/3"))));
        }

        [Test]
        public void SecondsLeftShouldGetSetAfterStarting()
        {
            Assert.That(stats.SecondsLeft, Is.EqualTo(10));
        }

        [Test]
        public void SecondsLeftShouldGetUpdated()
        {
            timeMock.AddSeconds(3);
            Thread.Sleep(100);

            Assert.That(stats.SecondsLeft, Is.EqualTo(7));
        }

        [Test]
        public void SecondsLeftShouldStartOverAfterTimeReached()
        {
            timeMock.AddSeconds(11);
            Thread.Sleep(100);

            Assert.That(stats.SecondsLeft, Is.EqualTo(10));
        }

        [Test]
        public void ShouldWaitForTimeOnSettingsBeforeSendingCommand()
        {
            classUnderTest.Stop();
            settings.SecondsBetweenVotes = 20;
            classUnderTest = new VoteTimer(stats, settings, restClientMock.Object, voteCache, timeMock);
            classUnderTest.Start();
            stats.AddForwardVote();

            timeMock.AddSeconds(11);
            Thread.Sleep(100);
            restClientMock.Verify(x => x.Execute(It.IsAny<RestRequest>()), Times.Never);

            timeMock.AddSeconds(10);
            Thread.Sleep(100);
            restClientMock.Verify(x => x.Execute(It.IsAny<RestRequest>()), Times.Once);
        }
    }

    internal class TimeMock : ITime
    {
        public TimeMock()
        {
            Now = DateTime.Now;
        }

        public DateTime Now { get; set; }
        public void AddSeconds(int seconds)
        {
            Now = Now.AddSeconds(seconds);
        }
    }
}