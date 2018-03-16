using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using TwitchPlaysMobiRobi.Web.Domain;

namespace TwitchPlaysMobiRobi.Web
{
  public class Startup
  {
    Stats stats;
    private VoteTimer timer;

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
      stats = new Stats();
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc();
      services.AddSingleton<Stats>(stats);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
    {
      applicationLifetime.ApplicationStopping.Register(Shutdown);
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }

      var stats = app.ApplicationServices.GetService<Stats>();
      timer = new VoteTimer(stats, new RestClient("http://google.com"), new Time());
      timer.Start();

      app.UseStaticFiles();

      app.UseMvc(routes =>
      {
        routes.MapRoute(
                  name: "default",
                  template: "{controller=Home}/{action=Index}/{id?}");
      });
    }

    private void Shutdown()
    {
        Console.WriteLine("Attempting to stop timer");
        timer.Stop();
    }
  }
}
