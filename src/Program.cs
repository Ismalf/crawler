// Packages

using System.Text;
using System.Text.RegularExpressions;
using Entities;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using script.Utilities;


namespace script;

public class Program
{

    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException("URL parameter expected"); // Url is a required parameter in case the page changes the domain
        }
        try
        {
            string currentDir = Directory.GetCurrentDirectory();
            var webSite = new Uri(args[0]);
            int capacity = 30;

            if (args.Length > 1 && !int.TryParse(args[1], out capacity)) // Parse and load new capacity (the page loads a max of 30 entries, but you can load less than that)
            {
                ConsoleUtilities.Print($"Could not set capacity to: '{args[1]}', keeping default capacity", ConsoleColor.Red);
            }

            ConsoleUtilities.Print($"Target DNS: {webSite.Authority}", ConsoleColor.Gray);
            ConsoleUtilities.Print($"Testing compliance (robots protocol)", ConsoleColor.Gray);

            using var client = new HttpClient
            {
                BaseAddress = webSite
            };

            var robots = await client.GetAsync("/robots.txt"); // Web Scraping can be seen as a harmful behavior towards a web site
            var robotsResponse = await robots.Content.ReadAsStringAsync();

            if (robotsResponse.Split('\n').Any(robot => robot.RemoveAfterSymbol('?').Equals($"Disallow: {webSite.AbsolutePath}"))) // so we check the robots protocol just to be sure we can legally scrape the requested site
            {
                ConsoleUtilities.Print("Neither scrapping nor crawling allowed for target domain due to robots protocol", ConsoleColor.DarkYellow);
                return;
            }
            else
            {
                ConsoleUtilities.Print("No restriction found, proceeding", ConsoleColor.DarkYellow);
            }

            var site = await client.GetAsync($"{webSite.AbsolutePath}");
            var siteStructure = await site.Content.ReadAsStringAsync();

            // load basic feature flags implementation from appsettings.json
            var root = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var parseHTML = bool.Parse(root.GetSection("features:parseHTML").Value ?? "false");
            if (parseHTML)
            {
                HtmlDocument htmlDocument = new();

                htmlDocument.LoadHtml(siteStructure);
                var extractData = bool.Parse(root.GetSection("features:extractData").Value ?? "false");
                if (extractData)
                {
                    var resultSubmission = htmlDocument.DocumentNode.GetByClass("submission", capacity).ToArray();
                    var resultSubtext = htmlDocument.DocumentNode.GetByClass("subtext", capacity).ToArray();
                    // due to the structure of the page we have to count the submission and comments section as separate nodes.
                    if (resultSubmission.Length != resultSubtext.Length) // there must be equal amount of submission and comment nodes
                    {
                        throw new Exception("Cannot determine full instances of submissions!");
                    }

                    List<NewsPost> newsPosts = [];
                    ConsoleUtilities.Print($"Parsing entries!", ConsoleColor.Gray);
                    for (int i = 0; i < resultSubmission.Length; i++)
                    {
                        var parsedSubmission = resultSubmission[i].ParseSubmission(); // retrieve data to a readable format
                        var parsedSubtext = resultSubtext[i].ParseSubtext(); // retrieve data to a readable format
                        newsPosts.Add(new(parsedSubmission, parsedSubtext)); // let the news post object decide what to do with raw retrieved data
                    }

                    ConsoleUtilities.Print($"Parsed successfully! Total parsed: {resultSubtext.Length}", ConsoleColor.Green);
                    var entries = new News(newsPosts);
                    var applyFilters = bool.Parse(root.GetSection("features:filter").Value ?? "false");
                    var appliedFilter = "none";
                    if (applyFilters)
                    {
                        if (args.Length < 3)
                        {
                            ConsoleUtilities.Print("No filter was provided for filter feature, no filter will be applied ...", ConsoleColor.DarkYellow);
                        }
                        else if (int.TryParse(args[2], out int filter))
                        {
                            switch (filter)
                            {
                                case 1: appliedFilter = "filter one"; entries.FilterOne(); break; // more than five words, ordered by comments
                                case 2: appliedFilter = "filter two"; entries.FilterTwo(); break; // less than or equal to five words, ordered by points
                                default: ConsoleUtilities.Print($"Unknown filter: ({args[2]}), filter not applied", ConsoleColor.Red); break;
                            }
                        }
                        else
                        {
                            ConsoleUtilities.Print($"Unknown filter: ({args[2]}), filter not applied", ConsoleColor.Red);
                        }

                    }
                    var exportCsv = bool.Parse(root.GetSection("features:exportToCsv").Value ?? "false");
                    if (exportCsv)
                    {
                        ConsoleUtilities.Print("Export to csv enabled, exporting ...", ConsoleColor.DarkBlue);
                        File.WriteAllText($"{currentDir}/results.csv", entries.ExportCsv(appliedFilter), Encoding.UTF8);
                    }
                    else
                    {
                        ConsoleUtilities.Print("Export to csv disabled, printing on screen ...", ConsoleColor.DarkBlue);
                        ConsoleUtilities.Print(entries.ToString(), ConsoleColor.DarkGray);
                    }
                }
            }
        }
        catch (UriFormatException ufe)
        {
            ConsoleUtilities.Print($"Format exception, could not parse the requested URL: {ufe.Message}", ConsoleColor.Red);
        }
        catch (Exception ex)
        {
            ConsoleUtilities.Print($"Unknown error occurred: {ex.Message}", ConsoleColor.Red);
        }
        return;
    }
}