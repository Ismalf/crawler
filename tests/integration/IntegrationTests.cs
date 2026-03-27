using System.Collections.Generic;
using script;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;
using System.Diagnostics;
using System;
using System.IO;
using System.Text;

namespace Tests.integration;

public class IntegrationTests(Xunit.Abstractions.ITestOutputHelper output)
{
    private readonly Xunit.Abstractions.ITestOutputHelper output = output;
    

    [Fact]
    public void End2End_GetAllPosts()
    {
        
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run https://news.ycombinator.com",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = false,
            WorkingDirectory =  Path.GetFullPath("../../../../../src")
        };


        using var process = new Process { StartInfo = processStartInfo };
        process.Start();
        process.WaitForExit();
        var csv = File.ReadAllLines(Path.GetFullPath("../../../../../src/results.csv"));
        Assert.Contains("Rank,Title,Points,Comments,Timestamp,Filter", csv);
        Assert.Equal(31, csv.Length);
    }

    [Fact]
    public void End2End_Get10Posts()
    {
        
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run https://news.ycombinator.com 10",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = false,
            WorkingDirectory =  Path.GetFullPath("../../../../../src")
        };
        using var process = new Process { StartInfo = processStartInfo };
        process.Start();
        process.WaitForExit();
        var csv = File.ReadAllLines(Path.GetFullPath("../../../../../src/results.csv"));
        Assert.Contains("Rank,Title,Points,Comments,Timestamp,Filter", csv);
        Assert.Equal(11, csv.Length);
    }

    [Fact]
    public void End2End_GetPostsFilterOne()
    {
        
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run https://news.ycombinator.com 10 1",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = false,
            WorkingDirectory =  Path.GetFullPath("../../../../../src")
        };
        using var process = new Process { StartInfo = processStartInfo };
        process.Start();
        process.WaitForExit();
        var csv = File.ReadAllText(Path.GetFullPath("../../../../../src/results.csv"));
        Assert.Contains(",filter one", csv);
    }

    [Fact]
    public void End2End_GetPostsFilterTwo()
    {
        
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run https://news.ycombinator.com 10 2",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = false,
            WorkingDirectory =  Path.GetFullPath("../../../../../src")
        };
        using var process = new Process { StartInfo = processStartInfo };
        process.Start();
        process.WaitForExit();
        output.WriteLine("path {0}", Path.GetFullPath("../../../../../src"));
        var csv = File.ReadAllText(Path.GetFullPath("../../../../../src/results.csv"));
        Assert.Contains(",filter two", csv);
        
    }
}
