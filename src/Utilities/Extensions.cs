using HtmlAgilityPack;

namespace script.Utilities;

public static class Extensions
{
    /// <summary>
    /// Recursive method to run through the HTML tree searching for a specific tag.
    /// In this scenario a tag is a css class name that has been previously identified in the target page.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="classes"></param>
    /// <param name="capacity">Limit capacity in case the page loads a big number of entries so there is no need to run trough every node</param>
    /// <returns></returns>
    public static List<HtmlNode> GetByClass(this HtmlNode parent, string classes, int capacity = 30)
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
                    var recursiveResult = node.GetByClass(classes);
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

    /// <summary>
    /// Submission nodes contain relevant information in a very specific structure, read that structure and retrieve data
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static string ParseSubmission(this HtmlNode node)
    {
        var rank = GetByClass(node, "rank").First().InnerText;
        var titleline = GetByClass(node, "titleline").First();
        var title = titleline.ChildNodes.First(cnode => cnode.Name.Equals("a")).InnerText;
        return $"{rank},{title}";
    }

    /// <summary>
    /// Submission nodes contain relevant information in a very specific structure, for comments and points that data is in the subtext node. Read that structure and retrieve data
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static string ParseSubtext(this HtmlNode node)
    {
        var score = GetByClass(node, "score").FirstOrDefault()?.InnerText ?? "0";
        var comments = node.ChildNodes.First().ChildNodes.FirstOrDefault(cnode => cnode.InnerText.Contains("comment"))?.InnerText ?? "0";
        return $"{score},{comments}";
    }

    /// <summary>
    /// Utility for any string, in this case its used to check for patterns on the robots.txt protocol
    /// </summary>
    /// <param name="text"></param>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static string RemoveAfterSymbol(this string text, char symbol)
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
}