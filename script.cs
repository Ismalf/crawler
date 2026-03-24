// Packages
#:package Microsoft.Extensions.Configuration.Json@10.0.5
#:package HtmlAgilityPack@1.12.4

using System.Reflection;
using System.Xml.Linq;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;

if (args.Length == 0)
{
    throw new ArgumentException("URL parameter expected");
}

try
{
    var webSite = new Uri(args[0]);


    Console.WriteLine($"Target DNS: {webSite.Authority}");
    Console.WriteLine($"Testing compliance (robots protocol)");

    var client = new HttpClient
    {
        BaseAddress = webSite
    };
    var robots = await client.GetAsync("/robots.txt");
    var robotsResponse = await robots.Content.ReadAsStringAsync();

    if (string.Empty.Equals(robotsResponse))
    {
        Console.WriteLine("No robots txt found, crawler is able to make a full scan");
    }
    else
    {
        Console.WriteLine("Robots txt found, crawler is forbidden access to specific resources");
    }



    var site = await client.GetAsync("/");
    var siteStructure = await site.Content.ReadAsStringAsync();

    // load basic feature flags implementation from appsettings.json
    var root = new ConfigurationBuilder().AddJsonFile("/workspaces/crawler/appsettings.json", false, true).Build();

    var parseHTML = bool.Parse(root.GetSection("features:parseHTML").Value ?? "false");
    if (parseHTML)
    {

        HtmlDocument htmlDocument = new();
        htmlDocument.LoadHtml(siteStructure);
        var extractData = bool.Parse(root.GetSection("features:extractData").Value ?? "false");
        if (extractData)
        {

            static List<HtmlNode> GetByClass(HtmlNode parent, string classes)
            {
                if (parent.GetClasses().Contains(classes))
                {
                    return [parent];
                }
                else if (parent.HasChildNodes)
                {
                    List<HtmlNode> result = [];
                    foreach (var node in parent.ChildNodes)
                    {
                        result.AddRange(GetByClass(node, classes));
                    }
                    return result;
                }
                else
                {
                    return [];
                }
            }

            static string ParseSubmission(HtmlNode node)
            {
                var rank = GetByClass(node, "rank").First().InnerText;
                var titleline = GetByClass(node, "titleline").First();
                var title = titleline.ChildNodes.First(cnode => cnode.Name.Equals("a")).InnerText;
                return $"{rank},{title}";
            }

            static string ParseSubtext(HtmlNode node)
            {
                var score = GetByClass(node, "score").First().InnerText;
                var comments = node.ChildNodes.First().ChildNodes.FirstOrDefault(cnode => cnode.InnerText.Contains("comment"))?.InnerText ?? "";
                return $"{score},{comments}";
            }


            var resultSubmission = GetByClass(htmlDocument.DocumentNode, "submission").ToArray();
            var resultSubtext = GetByClass(htmlDocument.DocumentNode, "subtext").ToArray();
            List<Entry> entriesList = [];
            Console.WriteLine($"Parsing entries!");
            for (int i = 0; i < resultSubmission.Length; i++)
            {
                var parsedSubmission = ParseSubmission(resultSubmission[i]);
                var parsedSubtext = ParseSubtext(resultSubtext[i]);
                entriesList.Add(new(parsedSubmission, parsedSubtext));

            }

            Console.WriteLine($"Parsed successfully! Total parsed: {resultSubtext.Length}");
            var entries = new Entries(entriesList);

            var applyFilters = bool.Parse(root.GetSection("features:filter").Value ?? "false");
            if (applyFilters)
            {
                if (args.Length == 1)
                {
                    Console.WriteLine("No filter was provided for filter feature");
                    Console.WriteLine("Skiping this feature");
                }
                else if (int.TryParse(args[1], out int filter))
                {
                    switch (filter)
                    {
                        case 1: entries.FilterOne(); break;
                        case 2: entries.FilterTwo(); break;
                        default: break;
                    }
                }
            }
            //save on db
        }
    }
}
catch (UriFormatException ufe)
{
    Console.WriteLine($"Format exception, could not parse the requested URL: {ufe.Message}");
}

class Entry
{
    string Rank { get; set; }
    string Title { get; set; }
    string Points { get; set; }
    string Comments { get; set; }

    public Entry(string submission, string subtext)
    {
        Rank = submission.Split(',')[0];
        Title = submission.Split(',')[1];
        Points = subtext.Split(',')[0].Split(' ')[0];
        Comments = subtext.Split(',')[1].Split('&')[0];
    }
}

class Entries
{
    List<Entry> Registry { get; set; } = [];

    public Entries(List<Entry> Registry)
    {
        this.Registry = Registry;
    }

    public void FilterOne()
    {

    }

    public void FilterTwo()
    {

    }
}