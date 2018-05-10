
using System;
using System.Net;
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
            webSiteRequest.Resource = settings.WebSiteBaseUrl + "voteResult";
            var websiteResponse = client.Execute<VoteResult>(webSiteRequest);
            if (websiteResponse.StatusCode == HttpStatusCode.OK)
            {
                if (websiteResponse.Data.Id != previousId)
                {
                    Console.WriteLine($"Got new command '{websiteResponse.Data.Vote}'");
                    previousId = websiteResponse.Data.Id;
                    var mobiRobiRequest = new RestRequest();
                    mobiRobiRequest.Resource = settings.MoboRobiBaseUrl + websiteResponse.Data.Vote;
                    var mobirobiResponse = client.Execute(mobiRobiRequest);
                    if (mobirobiResponse.StatusCode != HttpStatusCode.OK)
                    {
                        Console.WriteLine($"Got {mobirobiResponse.StatusCode} status code from Mobi Robi");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Got {websiteResponse.StatusCode} status code from web site");
            }
        }
    }

    public class VoteResult
    {
        public string Vote { get; set; }
        public string Id { get; set; }
    }
}