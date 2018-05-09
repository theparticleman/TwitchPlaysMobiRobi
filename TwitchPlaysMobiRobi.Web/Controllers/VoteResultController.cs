
using Microsoft.AspNetCore.Mvc;
using TwitchPlaysMobiRobi.Web.Domain;

namespace TwitchPlaysMobiRobi.Web
{
    public class VoteResultController : Controller
    {
        private readonly IVoteResultCache voteCache;

        public VoteResultController(IVoteResultCache voteCache)
        {
            this.voteCache = voteCache;
        }

        public JsonResult Index()
        {
            return Json(voteCache.Vote);
        }
    }
}