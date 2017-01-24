using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDesk.Options;

namespace NugetTest
{
    class Program
    {
        static void Main(string[] args)
        {
           var program = new Program();
            program.Execute(args);
        }

        void Execute(string[] args)
        {
            bool showHelp = false;
            var options = new OptionSet
            {
                { "url=", "Set the nuget feed url to test", u => _feedUrl = u },
                { "username=", "Set the nuget feed username if required", n => _feedUsername = n },
                { "password=", "Set the nuget feed password if required", n => _feedPassword = n },
                { "package=", "Set the nuget package Id to search for", v => _packageId = v },
                { "parallel=", "The number of simultaneous request threads", v => _parallel = int.Parse(v) },
                { "count=", "The number of requests each thread will make", v => _count = int.Parse(v) },
                { "h|help", "Show this message", _ => showHelp = true }
            };

            options.Parse(args);

            if (showHelp)
            {
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            var t = Enumerable.Range(0, _parallel)
               .AsParallel()
               .Select(threadId => Task.Run(() =>
               {
                   for (int i = 0; i < _count; i++)
                   {
                       Console.WriteLine($"Beginning request {i} for thread {threadId}");
                       var adaptor = new NugetAdaptor(_feedUrl, _feedUsername, _feedPassword);
                       adaptor.CreateReleaseTest(_packageId);
                       Console.WriteLine($"Finishing request {i} for thread {threadId}");
                   }
               }))
               .ToArray();

            Task.WaitAll(t);
        }

        string _feedUrl = "https://octopus.myget.org/F/octopus-dependencies/api/v2";
        string _feedUsername = "";
        string _feedPassword = "";
        string _packageId = "halibut";
        private int _parallel = 20;
        private int _count = 1;
    }
}