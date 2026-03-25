// Packages
#:package Microsoft.Extensions.Configuration.Json@10.0.5
#:package HtmlAgilityPack@1.12.4

using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;

#region Core Scrapping Logic

if (args.Length == 0)
{
    throw new ArgumentException("URL parameter expected");
}

try
{
    string currentDir = Directory.GetCurrentDirectory();
    var webSite = new Uri(args[0]);
    int capacity = 30;

    if (args.Length > 1 && !int.TryParse(args[1], out capacity))
    {
        Print($"Could not set capacity to: '{args[1]}', keeping default capacity", ConsoleColor.Red);
    }

    Print($"Target DNS: {webSite.Authority}", ConsoleColor.Gray);
    Print($"Testing compliance (robots protocol)", ConsoleColor.Gray);

    var client = new HttpClient
    {
        BaseAddress = webSite
    };

    var robots = await client.GetAsync("/robots.txt");
    var robotsResponse = await robots.Content.ReadAsStringAsync();

    if (robotsResponse.Split('\n').Any(robot => RemoveAfterSymbol(robot, '?').Equals($"Disallow: {webSite.AbsolutePath}")))
    {
        Print("Neither scrapping nor crawling allowed for target domain due to robots protocol", ConsoleColor.DarkYellow);
        return 0;
    }
    else
    {
        Print("No restriction found, proceeding", ConsoleColor.DarkYellow);
    }

    var site = await client.GetAsync($"{webSite.AbsolutePath}");
    var siteStructure = await site.Content.ReadAsStringAsync();

    // load basic feature flags implementation from appsettings.json
    var root = new ConfigurationBuilder().AddJsonFile($"{currentDir}/appsettings.json", false, true).Build();

    var parseHTML = bool.Parse(root.GetSection("features:parseHTML").Value ?? "false");
    if (parseHTML)
    {
        HtmlDocument htmlDocument = new();

        htmlDocument.LoadHtml(siteStructure);
        var extractData = bool.Parse(root.GetSection("features:extractData").Value ?? "false");
        if (extractData)
        {
            var resultSubmission = GetByClass(htmlDocument.DocumentNode, "submission", capacity).ToArray();
            var resultSubtext = GetByClass(htmlDocument.DocumentNode, "subtext", capacity).ToArray();

            if (resultSubmission.Length != resultSubtext.Length)
            {
                throw new Exception("Cannot determine full instances of submissions!");
            }

            List<NewsPost> newsPosts = [];
            Print($"Parsing entries!", ConsoleColor.Gray);
            for (int i = 0; i < resultSubmission.Length; i++)
            {
                var parsedSubmission = ParseSubmission(resultSubmission[i]);
                var parsedSubtext = ParseSubtext(resultSubtext[i]);
                newsPosts.Add(new(parsedSubmission, parsedSubtext));
            }

            Print($"Parsed successfully! Total parsed: {resultSubtext.Length}", ConsoleColor.Green);
            var entries = new News(newsPosts);
            var applyFilters = bool.Parse(root.GetSection("features:filter").Value ?? "false");
            var appliedFilter = "none";
            if (applyFilters)
            {
                if (args.Length < 3)
                {
                    Print("No filter was provided for filter feature, no filter will be applied ...", ConsoleColor.DarkYellow);
                }
                else if (int.TryParse(args[2], out int filter))
                {
                    switch (filter)
                    {
                        case 1: appliedFilter = "filter one"; entries.FilterOne(); break;
                        case 2: appliedFilter = "filter two"; entries.FilterTwo(); break;
                        default: Print($"Unknown filter: ({args[2]}), filter not applied", ConsoleColor.Red); break;
                    }
                }
                else
                {
                    Print($"Unknown filter: ({args[2]}), filter not applied", ConsoleColor.Red);
                }

            }
            var exportCsv = bool.Parse(root.GetSection("features:exportToCsv").Value ?? "false");
            if (exportCsv)
            {
                Print("Export to csv enabled, exporting ...", ConsoleColor.DarkBlue);
                File.WriteAllText($"{currentDir}/results.csv", entries.ExportCsv(appliedFilter), Encoding.UTF8);
            }
            else
            {
                Print("Export to csv disabled, printing on screen ...", ConsoleColor.DarkBlue);
                Print(entries.ToString(), ConsoleColor.DarkGray);
            }
        }
    }
}
catch (UriFormatException ufe)
{
    Print($"Format exception, could not parse the requested URL: {ufe.Message}", ConsoleColor.Red);
    return 1;
}
catch (Exception ex)
{
    Print($"Unknown error ocurred> {ex.Message}", ConsoleColor.Red);
    return 1;
}

return 0;

#endregion

#region Functions

static void Print(string message, ConsoleColor color)
{
    Console.ForegroundColor = color;
    Console.WriteLine(message);
    Console.ResetColor();
}

static List<HtmlNode> GetByClass(HtmlNode parent, string classes, int capacity = 30)
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
            // Do not search for more than estimated capacity
            if (result.Count < capacity)
            {
                var recursiveResult = GetByClass(node, classes);
                if (capacity - result.Count - recursiveResult.Count == 0)
                {
                    result.AddRange(recursiveResult);
                }
                else
                {
                    result.AddRange(recursiveResult.Take(capacity - result.Count));
                }
            }
            else
            {
                return result;
            }

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
    var score = GetByClass(node, "score").FirstOrDefault()?.InnerText ?? "0";
    var comments = node.ChildNodes.First().ChildNodes.FirstOrDefault(cnode => cnode.InnerText.Contains("comment"))?.InnerText ?? "0";
    return $"{score},{comments}";
}

static string RemoveAfterSymbol(string text, char symbol)
{
    if (string.IsNullOrEmpty(text))
        return text;

    int index = text.IndexOf(symbol);
    if (index >= 0)
    {
        return text.Substring(0, index); // Keep everything before the symbol
    }

    return text; // Symbol not found
}

#endregion

#region Classes

class NewsPost
{
    public string Rank { get; set; }
    public string Title { get; set; }
    public int Points { get; set; }
    public int Comments { get; set; }

    public NewsPost(string submission, string subtext)
    {
        Rank = submission.Split(',')[0].Replace('.', ' ').Trim();
        Title = submission.Split(',')[1].Trim();
        Points = int.Parse(subtext.Split(',')[0].Split(' ')[0]);
        Comments = int.Parse(subtext.Split(',')[1].Split('&')[0]);
    }

    public override string ToString()
    {
        return string.Format("{0,-6} | {1,-150} | {2,-7} | {3,-9}", Rank, Title, Points, Comments);
    }

    public string CsvFormat()
    {
        return string.Format("{0},\"{1}\",{2},{3}", Rank, Title, Points, Comments);
    }
}

class News(List<NewsPost> newsList)
{
    public List<NewsPost> Registry { get; set; } = newsList;

    public void FilterOne()
    {
        Registry = [.. Registry.Where(reg => RemoveSpecialCharacters(reg.Title).Split(" ").Length > 5).OrderBy(reg => reg.Comments)];
    }

    public void FilterTwo()
    {
        Registry = [.. Registry.Where(reg => RemoveSpecialCharacters(reg.Title).Split(" ").Length <= 5).OrderBy(reg => reg.Points)];
    }

    private static string RemoveSpecialCharacters(string input)
    {
        if (input == null)
            return string.Empty;

        // Pattern: keep only a-z, A-Z, 0-9, space, underscore, dash
        return Regex.Replace(input, @"[^a-zA-Z0-9 ]", "");
    }

    public override string ToString()
    {
        return string.Join('\n', [string.Format("{0,-6} | {1,-150} | {2,-7} | {3,-9}", "Rank", "Title", "Points", "Comments"), .. Registry.Select(reg => reg.ToString())]);
    }

    public string ExportCsv(string filter)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        return string.Join('\n', [string.Format("{0},{1},{2},{3},{4},{5}", "Rank", "Title", "Points", "Comments", "Timestamp", "Filter"), .. Registry.Select(reg => string.Format("{0},{1},{2}", reg.CsvFormat(), timestamp, filter))]);
    }
}

#endregion