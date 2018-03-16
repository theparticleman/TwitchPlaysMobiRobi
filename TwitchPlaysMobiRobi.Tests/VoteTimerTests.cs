using System;
using System.Threading;
using Moq;
using NUnit.Framework;
using RestSharp;
using TwitchPlaysMobiRobi.Web.Domain;

namespace TwitchPlaysMobiRobi.Tests
{
  public class VoteTimerTests
  {
    private Stats stats;
    private Mock<IRestClient> restClientMock;
    private TimeMock timeMock;
    private VoteTimer classUnderTest;

    [SetUp]
    public void Setup()
    {
      stats = new Stats();
      restClientMock = new Mock<IRestClient>();
      timeMock = new TimeMock();
      classUnderTest = new VoteTimer(stats, restClientMock.Object, timeMock);
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
      restClientMock.Verify(x => x.Execute(It.Is<RestRequest>(r => r.Resource.Contains("left"))));
    }

    [Test]
    public void WhenRightHasMostVotes()
    {
      stats.AddRightVote();
      stats.AddRightVote();
      stats.AddLeftVote();
      timeMock.AddSeconds(11);
      Thread.Sleep(100);
      restClientMock.Verify(x => x.Execute(It.Is<RestRequest>(r => r.Resource.Contains("right"))));
    }

    [Test]
    public void WhenForwardHasMostVotes()
    {
      stats.AddForwardVote();
      stats.AddForwardVote();
      stats.AddRightVote();
      timeMock.AddSeconds(11);
      Thread.Sleep(100);
      restClientMock.Verify(x => x.Execute(It.Is<RestRequest>(r => r.Resource.Contains("forward"))));
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