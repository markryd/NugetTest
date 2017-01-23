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
                { "h|help", "Show this message", _ => showHelp = true }
            };

            options.Parse(args);

            if (showHelp)
            {
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            var t = Enumerable.Range(0, 20)
               .AsParallel()
               .Select(_ => Task.Run(() =>
               {
                   var adaptor = new NugetAdaptor(_feedUrl, _feedUsername, _feedPassword);
                   adaptor.CreateReleaseTest(_packageId);
               }))
               .ToArray();

            Task.WaitAll(t);
        }

        string _feedUrl = "https://octopus.myget.org/F/octopus-dependencies/api/v2";
        string _feedUsername = "";
        string _feedPassword = "";
        string _packageId = "halibut";
    }
}