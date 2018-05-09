using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TwitchPlaysMobiRobi.Web.Models;
using TwitchPlaysMobiRobi.Domain;

namespace TwitchPlaysMobiRobi.Web.Controllers
{
  public class MController : Controller
  {
    private Stats stats;

    public MController(Stats stats)
    {
        this.stats = stats;
    }

    [HttpGet]
    public IActionResult Index()
    {
      return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Index(string vote)
    {
      if (string.IsNullOrEmpty(vote)) return View();
      switch (vote)
      {
        case "stop":
          stats.AddStopVote();
          break;
        case "left":
          stats.AddLeftVote();
          break;
        case "right":
          stats.AddRightVote();
          break;
        case "forward":
          stats.AddForwardVote();
          break;
      }
      return View((object)vote);
    }
  }
}
