using System.Collections.Generic;
using NUnit.Framework;

public class TestBot
{
    private readonly PB3 bot3 = new();

    private readonly PB4 bot4 = new();
    private readonly BrucesBot brucesBot = new();
    private readonly EnhancedWordCombinator enhancedWordCombinator = new();

    [Test]
    public void TestDictionary()
    {
        BrucesDicionaryTrie brucesDictionaryTrie = new();
        brucesDictionaryTrie.LoadDictionary("C:/Users/bacon/Downloads/dictionary-EN.txt");
        // Remaining: 'e'. Rack: 'e', 'a', 'r'. Passes.
        var wordList = new List<string> { "POT" };
        var rack = new List<string> { "TCLIWNU" };

        var x = ""; // brucesBot.FindAllWords(wordList, rack);
        Assert.IsTrue(x != null,
            "Simple test failed. Expected true: lop -> lope (remaining 'e') in ear.");
    }

    [Test]
    public void TestBrucesBot()
    {
        // Remaining: 'e'. Rack: 'e', 'a', 'r'. Passes.
        var wordList = new List<string> { "POT" };
        var rack = new List<string> { "TCLIWNU" };

        var x = brucesBot.FindAllWords(wordList, rack);
        Assert.IsTrue(x != null,
            "Simple test failed. Expected true: lop -> lope (remaining 'e') in ear.");
    }

    [Test]
    public void TestPb3()
    {
        // Remaining: 'e'. Rack: 'e', 'a', 'r'. Passes.
        var wordList = new List<string> { "POT" };
        var rack = new List<string> { "TCLIWNU" };

        var x = bot3.FindHighestScoringWord(wordList, rack);
        Assert.IsTrue(x != null,
            "Simple test failed. Expected true: lop -> lope (remaining 'e') in ear.");
    }


    [Test]
    public void TestPb4NoRack()
    {
        // Remaining: 'e'. Rack: 'e', 'a', 'r'. Passes.
        var wordList = new List<string> { "POT" };
        var rack = new List<string> { "" };

        var x = bot4.GenerateCombinations("CAT", "R");
        Assert.IsTrue(x != null,
            "Simple test failed. Expected true: lop -> lope (remaining 'e') in ear.");
    }

/* */
    [Test]
    public void TestCombinator()
    {
        // Remaining: 'e'. Rack: 'e', 'a', 'r'. Passes.
        var wordList = new List<string> { "POT" };
        var rack = new List<string> { "TCLIWNU" };

        var x = enhancedWordCombinator.GenerateCombinations("CAT", "OR");
        Assert.IsTrue(x != null,
            "Simple test failed. Expected true: lop -> lope (remaining 'e') in ear.");
    }
}