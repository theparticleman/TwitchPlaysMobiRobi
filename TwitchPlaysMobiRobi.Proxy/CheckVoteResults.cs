
using System;
using RestSharp;

namespace TwitchPlaysMobiRobi.Proxy
{
    public class CheckVoteResults
    {
        private readonly IRestClient client;
        private readonly Settings settings;
        private string previousId;

        public CheckVoteResults(IRestClient client, Settings settings)
        {
            this.client = client;
            this.settings = settings;
        }

        public void CheckVote()
        {
            var webSiteRequest = new RestRequest();
            webSiteRequest.Resource = settings.WebSiteBaseUrl + "vote";
            var result = client.Execute<VoteResult>(webSiteRequest);
            if (result.Data.Id != previousId)
            {
                previousId = result.Data.Id;
                var mobiRobiRequest = new RestRequest();
                mobiRobiRequest.Resource = settings.MoboRobiBaseUrl + result.Data.Vote;
                client.Execute(mobiRobiRequest);
            }
        }
    }

    public class VoteResult
    {
        public string Vote { get; set; }
        public string Id { get; set; }
    }
}