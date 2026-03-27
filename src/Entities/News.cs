using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.FileProviders;

namespace Entities;

/// <summary>
/// List of news posts
/// </summary>
/// <param name="newsList"></param>
public class News(List<NewsPost> newsList)
{
    public List<NewsPost> Registry { get; set; } = newsList;

    /// <summary>
    /// Filter all previous entries with more than five words in the title, ordered by the number of comments first.
    /// </summary>
    public void FilterOne()
    {
        Registry = [.. Registry.Where(reg => reg.CountTitleWords() > 5).OrderBy(reg => reg.Comments)];
    }

    /// <summary>
    /// Filter all previous entries with less than or equal to five words in the title, ordered by points.
    /// </summary>
    public void FilterTwo()
    {
        Registry = [.. Registry.Where(reg => reg.CountTitleWords() <= 5).OrderBy(reg => reg.Points)];
    }

    /// <summary>
    /// Custom override to print a table on console
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return string.Join('\n', [string.Format("{0,-6} | {1,-150} | {2,-7} | {3,-9}", "Rank", "Title", "Points", "Comments"), .. Registry.Select(reg => reg.ToString())]);
    }

    /// <summary>
    /// Custom export method, string builder is more efficient
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    public string ExportCsv(string filter)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        var sb = new StringBuilder();
        sb.AppendLine("Rank,Title,Points,Comments,Timestamp,Filter");
        foreach (var reg in Registry)
        {
            sb.AppendLine($"{reg.CsvFormat()},{timestamp},{filter}");
        }
        return sb.ToString();
    }
}