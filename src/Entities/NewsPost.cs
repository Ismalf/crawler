using System.Text.RegularExpressions;

namespace Entities;

/// <summary>
/// Represents a single entry on news.ycombinator.com
/// </summary>
public class NewsPost
{
    private static readonly Regex NonNumericRegex = new(@"[^0-9]", RegexOptions.Compiled);
    private static readonly Regex SpecialCharsRegex = new(@"[^a-zA-Z0-9 ]", RegexOptions.Compiled);

    public int Rank { get; set; }
    public string Title { get; set; }
    public int Points { get; set; }
    public int Comments { get; set; }

    public NewsPost(string submission, string subtext)
    {
        if (!int.TryParse(NonNumericRegex.Replace(submission.Split(',')[0], "") ?? "0", out int rankClean))
        {
            rankClean = 0;
        }
        if (!int.TryParse(NonNumericRegex.Replace(subtext.Split(',')[0], "") ?? "0", out int pointsClean))
        {
            pointsClean = 0;
        }
        if (!int.TryParse(NonNumericRegex.Replace(subtext.Split(',')[1], "") ?? "0", out int commentsClean))
        {
            commentsClean = 0;
        }

        Rank = rankClean;
        Title = submission.Split(',')[1].Trim();
        Points = pointsClean;
        Comments = commentsClean;
    }

    /// <summary>
    /// Custom override to print a table on the console
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return string.Format("{0,-6} | {1,-150} | {2,-7} | {3,-9}", Rank, Title, Points, Comments);
    }


    /// <summary>
    /// Custom method to export data
    /// </summary>
    /// <returns></returns>
    public string CsvFormat()
    {
        return string.Format("{0},\"{1}\",{2},{3}", Rank, Title, Points, Comments);
    }

    /// <summary>
    /// Class utility to count words on the title
    /// </summary>
    /// <returns></returns>
    public int CountTitleWords()
    {
        return CountWords(RemoveSpecialCharacters(Title));
    }

    /// <summary>
    /// Class utility to sanitize input
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private static string RemoveSpecialCharacters(string input)
    {
        if (input == null)
            return string.Empty;

        // Pattern: keep only a-z, A-Z, 0-9, space, underscore, dash
        return SpecialCharsRegex.Replace(input, "");
    }

    /// <summary>
    /// Remove special characters and count amount of words.
    /// When counting words, consider only the spaced words and exclude any symbols. For instance, the phrase “This is - a self-explained example” should be counted as having 5 words.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private static int CountWords(string input)
    {
        return RemoveSpecialCharacters(input).Trim().Split(" ").Where(word => !word.Equals("")).ToArray().Length; // account for empty spaces after input sanitization method
    }
}