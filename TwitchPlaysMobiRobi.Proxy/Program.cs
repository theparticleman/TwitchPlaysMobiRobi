using System;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;

namespace TwitchPlaysMobiRobi.Proxy
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = GetSettings(args);
            var checkVoteResults = new CheckVoteResults(new RestClient("http://google.com"), settings);
            var running = true;
            var task = Task.Run(() =>
            {
                while (running)
                {
                    checkVoteResults.CheckVote();
                    Task.Delay(100);
                }
            });

            Console.WriteLine("Press enter to quit");
            Console.ReadLine();

            running = false;
            Thread.Sleep(1000);
            task.Dispose();
        }

        private static Settings GetSettings(string[] args)
        {
            var settings = new Settings();
            if (args.Length == 2)
            {
                settings.WebSiteBaseUrl = args[0];
                settings.MoboRobiBaseUrl = args[1];
            }
            else
            {
                Console.Write("Enter web site url: ");
                settings.WebSiteBaseUrl = Console.ReadLine();
                Console.Write("Enter mobi robi url: ");
                settings.MoboRobiBaseUrl = Console.ReadLine();
            }

            return settings;
        }
    }
}
