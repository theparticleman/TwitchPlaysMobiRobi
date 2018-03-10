using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TwitchPlaysMobiRobi.Web.Models;

namespace TwitchPlaysMobiRobi.Web.Controllers
{
  public class MController : Controller
  {
    [HttpGet]
    public IActionResult Index()
    {
      return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Index(string vote)
    {
      if (string.IsNullOrEmpty(vote)) return View();
      return Content("Thanks for voting: " + vote);
    }
  }
}
