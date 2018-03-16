using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TwitchPlaysMobiRobi.Web.Models;
using TwitchPlaysMobiRobi.Web.Domain;

namespace TwitchPlaysMobiRobi.Web.Controllers
{
  public class StatsController : Controller
  {
    private readonly Stats stats;

    public StatsController(Stats stats)
    {
        this.stats = stats;
    }
    public JsonResult Index()
    {
      return Json(stats);
    }
  }
}
