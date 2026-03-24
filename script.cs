// Script-style program to scrap a web site

using System;
using System.Text;

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

    Console.WriteLine(robotsResponse);

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

    Console.WriteLine(siteStructure);

}
catch (UriFormatException ufe)
{
    Console.WriteLine($"Format exception, could not parse the requested URL: {ufe.Message}");
}