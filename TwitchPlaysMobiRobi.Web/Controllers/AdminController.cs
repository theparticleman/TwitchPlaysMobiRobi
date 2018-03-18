using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TwitchPlaysMobiRobi.Web.Domain;

namespace TwitchPlaysMobiRobi.Web.Controllers
{
  public class AdminController : Controller
  {
    private readonly Settings settings;

    public AdminController(Settings settings)
    {
      this.settings = settings;
    }

    public IActionResult Index()
    {
      return View(settings);
    }

    [HttpPost]
    public IActionResult BaseUrl(string url)
    {
      Console.WriteLine($"Setting base url to '{url}'");
      settings.MobiRobiBaseUrl = url;
      return Redirect("/Admin");
    }

    [HttpPost]
    public IActionResult SecondsBetweenVotes(int seconds)
    {
      Console.WriteLine($"Setting seconds between votes to {seconds}");
      settings.SecondsBetweenVotes = seconds;
      return Redirect("/Admin");
    }
  }
}
