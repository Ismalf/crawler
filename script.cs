// Packages
#:package Microsoft.Extensions.Configuration.Json@10.0.5
#:package HtmlAgilityPack@1.12.4

using System;
using System.Text;
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
        static int CountNodes(HtmlNode node)
        {
            if (node.HasChildNodes)
            {
                int nodes = 0;
                foreach (var childNode in node.ChildNodes)
                {
                    nodes += CountNodes(childNode);
                }
                return nodes;
            }
            else
            {
                return 1;
            }
        }
        HtmlDocument htmlDocument = new();
        htmlDocument.LoadHtml(siteStructure);
        Console.WriteLine($"HTML Parsed! Total nodes in document: {CountNodes(htmlDocument.DocumentNode)}");
    }
}
catch (UriFormatException ufe)
{
    Console.WriteLine($"Format exception, could not parse the requested URL: {ufe.Message}");
}