using System.Collections.Generic;
using Entities;
using Xunit;

namespace Tests.unit;

public class UnitTests(Xunit.Abstractions.ITestOutputHelper output)
{
    private readonly Xunit.Abstractions.ITestOutputHelper output = output;

    [Fact]
    public void TestParsing()
    {
        var post = new NewsPost("1,Hello world", "10 points,5 comments");

        Assert.Equal(1, post.Rank);
        Assert.Equal("Hello world", post.Title);
        Assert.Equal(10, post.Points);
        Assert.Equal(5, post.Comments);
    }
    [Fact]
    public void TestParsing2()
    {
        var post = new NewsPost("1.,Mama mia - this a test indeed", "100000 points,50&nbspcomments");

        Assert.Equal(1, post.Rank);
        Assert.Equal("Mama mia - this a test indeed", post.Title);
        Assert.Equal(100000, post.Points);
        Assert.Equal(50, post.Comments);
    }

    [Fact]
    public void TestParsing3()
    {
        var post = new NewsPost("1.,", "100000 points,50&nbspcomments");

        Assert.Equal(1, post.Rank);
        Assert.Equal("", post.Title);
        Assert.Equal(100000, post.Points);
        Assert.Equal(50, post.Comments);
    }

    [Fact]
    public void TestParsing4()
    {
        var post = new NewsPost("1.,", "points,&nbspcomments");

        Assert.Equal(1, post.Rank);
        Assert.Equal("", post.Title);
        Assert.Equal(0, post.Points);
        Assert.Equal(0, post.Comments);
    }

    [Fact]
    public void TestFilterOne()
    {
        var posts = new List<NewsPost>
        {
            new("1,short", "10 points,5 comments"),
            new("2,this has five words here", "10 points,2 comments"),
            new("3,this has exactly five words", "10 points,1 comments"),
            new("4,this has exactly - five words", "10 points,1 comments"),
            new("5,this is exactly - how we $ test", "10 points,0 comments"),
            new("6,hi          -     whats       up", "10 points,0 comments"),
            new("7,hi          -              ", "10 points,0 comments"),
            new("8,          -              ", "10 points,0 comments"),
        };
        var news = new News(posts);

        news.FilterOne();


        Assert.Single(news.Registry);
        Assert.Equal(5, news.Registry[0].Rank);
    }

    [Fact]
    public void TestFilterOne2()
    {
        var posts = new List<NewsPost>
        {
            new("1,this has six full words ok!", "1 points,50 comments"),
            new("2,this has six full words ok!", "2 points,2 comments"),
            new("3,this has six full words ok!", "3 points,0 comments"),
            new("4,this has six full words ok!", "4 points,13 comments"),
            new("5,this has six full words ok!", "5 points,1 comments"),
            new("6,hi          -     whats       up", "10 points,5 comments"),
            new("7,hi          -              ", "6 points,7 comments"),
            new("8,this has six full words ok!", "7 points,8 comments"),
        };
        var news = new News(posts);

        news.FilterOne();

        

        Assert.Equal(3, news.Registry[0].Rank);
        Assert.Equal(5, news.Registry[1].Rank);
        Assert.Equal(2, news.Registry[2].Rank);
        Assert.Equal(8, news.Registry[3].Rank);
        Assert.Equal(4, news.Registry[4].Rank);
        Assert.Equal(1, news.Registry[5].Rank);
    }

    [Fact]
    public void TestFilterTwo()
    {
        var posts = new List<NewsPost>
        {
            new("1,hello world", "10 points,1 comments"),
            new("2,short text", "25 points,2 comments"),
            new("3,this has five word ok!", "250 points,3 comments"),
            new("4,this has more than five words indeed", "15 points,4 comments")
        };
        var news = new News(posts);

        news.FilterTwo();

        Assert.Equal(3, news.Registry.Count);
        Assert.Equal(1, news.Registry[0].Rank);
        Assert.Equal(2, news.Registry[1].Rank);
        Assert.Equal(3, news.Registry[2].Rank);
    }

    [Fact]
    public void TestFilterTwo2()
    {
        var posts = new List<NewsPost>
        {
            new("1,hello world this ", "5 points,1 comments"),
            new("2,hello world that             a", "4 points,2 comments"),
            new("3,hello world this has a lot of wordst", "3 points,3 comments"),
            new("4,short text       -    - . + =                  ha ha ha ", "2 points,4 comments"),
            new("5,this has more than five words indeed", "1 points,5 comments")
        };
        var news = new News(posts);

        news.FilterTwo();

        Assert.Equal(3, news.Registry.Count);
        Assert.Equal(4, news.Registry[0].Rank);
        Assert.Equal(1, news.Registry[2].Rank);
    }
}