using System.Collections.Generic;
using NUnit.Framework;

public class TestBot
{
    private readonly SaveMyBot bot3 = new();

    private readonly SaveMyBot bot4 = new();

    private readonly SaveMyBot brucesBot = new();
    // private readonly EnhancedWordCombinator enhancedWordCombinator = new();

    [Test]
    public void TestDictionary()
    {
        TrieDictionary dictionaryTrieDictionary = new();
        dictionaryTrieDictionary.LoadDictionary("C:/Users/bacon/Downloads/dictionary-EN.txt");
        // Remaining: 'e'. Rack: 'e', 'a', 'r'. Passes.
        var wordList = new List<string> { "POT" };
        var rack = new List<string> { "TCLIWNU" };

        var x = ""; // brucesBot.FindAllWords(wordList, rack);
        Assert.IsTrue(x != null,
            "Simple test failed. Expected true: lop -> lope (remaining 'e') in ear.");
    }

//     [Test]
//     public void TestBrucesBot()
//     {
//         // Remaining: 'e'. Rack: 'e', 'a', 'r'. Passes.
//         var wordList = new List<string> { "POT" };
//         var rack = new List<string> { "TCLIWNU" };
//
//        // var x = brucesBot.FindAllWords(wordList, rack);
//         Assert.IsTrue(x != null,
//             "Simple test failed. Expected true: lop -> lope (remaining 'e') in ear.");
//     }
//
//     [Test]
//     public void TestPb3()
//     {
//         // Remaining: 'e'. Rack: 'e', 'a', 'r'. Passes.
//         var wordList = new List<string> { "POT" };
//         var rack = new List<string> { "TCLIWNU" };
//
//   //      var x = bot3.FindHighestScoringWord(wordList, rack);
//         Assert.IsTrue(x != null,
//             "Simple test failed. Expected true: lop -> lope (remaining 'e') in ear.");
//     }
//
//
//     [Test]
//     public void TestPb4NoRack()
//     {
//         // Remaining: 'e'. Rack: 'e', 'a', 'r'. Passes.
//         var wordList = new List<string> { "POT" };
//         var rack = new List<string> { "" };
//
//   //      var x = bot4.GenerateCombinations("CAT", "R");
//         Assert.IsTrue(x != null,
//             "Simple test failed. Expected true: lop -> lope (remaining 'e') in ear.");
//     }
//
//     [Test]
//     public void TestWordGenerator()
//     {
//         // 1. Initialize the dictionary (mocked for this example)
//         //  var dictionary = new WordGenerator.TrieDictionary();
//         var dictionary = new TrieDictionary();
//         dictionary.LoadDictionary();
//         var generator = new WordGenerator(dictionary);
//
//         // 2. Define the words and constraints
//         var originalWord = "XXX"; //"CAT";
//         var rackWord = "SROLKE";
//         var minLettersForFallback = 4; // Words must be at least 4 letters long in Phase 2.
//
//         // 3. Define the options
//         var options = new WordGenerator.GenerationOptions
//         {
//             MaxWordsToCreate = 500, // Stop after 5 words
//             MaxLength = 800, // Max word length of 8
//             StopAfterFirstWord = false, // Find up to 5 words
//             MaxLettersToAdd = 7 // Only allow adding up to 2 rack letters (max length 3+2=5)
//         };
//
//         Debug.Log($"Original Word: {originalWord}, Rack Word: {rackWord}");
//         Debug.Log(
//             $"Options: Max Words: {options.MaxWordsToCreate}, Max Length: {options.MaxLength}, Max Added: {options.MaxLettersToAdd}");
//
//         // 4. Generate the words
//         var foundWords = generator.GenerateWords(originalWord, rackWord, minLettersForFallback, options);
//
//         // 5. Output results
//         Debug.Log("\n--- FINAL RESULTS ---");
//         if (foundWords.Any())
//         {
//             Debug.Log($"Found {foundWords.Count} valid word(s):");
//             foreach (var word in foundWords)
//             {
//                 Debug.Log($"- {word}");
//             }
//         }
//         else
//         {
//             Debug.Log("No words found matching the criteria.");
//         } /* */
//     }
//
//     [Test]
//     public void TestWordGeneratorList()
//     {
//         // 1. Initialize the dictionary (mocked for this example)
//         //  var dictionary = new WordGenerator.TrieDictionary();
//         var dictionary = new TrieDictionary();
//         dictionary.LoadDictionary();
//         var generator = new WordGenerator(dictionary);
//
//         // 2. Define the words and constraints
//         var originalWords = new List<string> { "TEE", "XXX" };
//         var rackWord = "SROLKE";
//         var minLettersForFallback = 4; // Words must be at least 4 letters long in Phase 2.
//
//         // 3. Define the options
//         var options = new WordGenerator.GenerationOptions
//         {
//             MaxWordsToCreate = 100, // Stop after 5 words
//             MaxLength = 95, // Max word length of 8
//             StopAfterFirstWord = false, // Find up to 5 words
//             MaxLettersToAdd = 7 // Only allow adding up to 2 rack letters (max length 3+2=5)
//         };
//
//         Debug.Log($"Original Word: {originalWords}, Rack Word: {rackWord}");
//         Debug.Log(
//             $"Options: Max Words: {options.MaxWordsToCreate}, Max Length: {options.MaxLength}, Max Added: {options.MaxLettersToAdd}");
//
//         // 4. Generate the words
//         var foundWords = generator.GenerateWords(originalWords, rackWord, minLettersForFallback, options);
//
//         // 5. Output results
//         Debug.Log("\n--- FINAL RESULTS ---");
//         if (foundWords.Any())
//         {
//             Debug.Log($"Found {foundWords.Count} valid word(s):");
//             foreach (var word in foundWords)
//             {
//                 Debug.Log($"- {word}");
//             }
//         }
//         else
//         {
//             Debug.Log("No words found matching the criteria.");
//         } /* */
//     }
//
// /*
//     [Test]
//     public void TestCombinator()
//     {
//         // Remaining: 'e'. Rack: 'e', 'a', 'r'. Passes.
//         var wordList = new List<string> { "POT" };
//         var rack = new List<string> { "TCLIWNU" };
//
//         var x = enhancedWordCombinator.GenerateCombinations("CAT", "OR");
//         Assert.IsTrue(x != null,
//             "Simple test failed. Expected true: lop -> lope (remaining 'e') in ear.");
//     }*/
}