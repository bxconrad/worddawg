public class TestWordChecker
{
    // private readonly WordChecker wordChecker = new(); //meObject().AddComponent<WordChecker>();
    //
    // [Test]
    // public void SimpleTest_ResultTrue()
    // {
    //     // Remaining: 'e'. Rack: 'e', 'a', 'r'. Passes.
    //     var original = "lop";
    //     var @new = "lope";
    //     var rack = "ear";
    //
    //     Assert.IsTrue(wordChecker.CanFormNewWord(original, @new, rack),
    //         "Simple test failed. Expected true: lop -> lope (remaining 'e') in ear.");
    // }
    //
    // [Test]
    // public void SimpleTest_ResultFalse()
    // {
    //     // Remaining: 'e'. Rack: 'e', 'a', 'r'. Passes.
    //     var original = "lop";
    //     var @new = "lope";
    //     var rack = "car";
    //
    //     Assert.IsFalse(wordChecker.CanFormNewWord(original, @new, rack),
    //         "Simple test failed. Expected false: lop -> lope (no remaining 'e') in car.");
    // }
    //
    // [Test]
    // // Original word is loo, new word is loot, rack word is tar
    // public void TestDoubleLettersInOriginalWord_ResultTrue()
    // {
    //     // Original needs two 'o'. New has two 'o'. (Passes Step 1)
    //     // New word has one 't' not required by original. Remaining: 't'. Rack: 't', 'a', 'r'. Passes.
    //     var original = "loo";
    //     var @new = "loot";
    //     var rack = "tar";
    //
    //     Assert.IsTrue(wordChecker.CanFormNewWord(original, @new, rack),
    //         "Double letters in original test failed. Expected true: loo -> loot (remaining 't') in tar.");
    // }
    //
    // [Test] // Original word is Loo, new word is lot. Rack word is tar
    // public void TestDoubleLettersInOriginalButNotNewWord_ResultFalse()
    // {
    //     // Original needs two 'o'. New word has only one 'o'. (Fails Step 1)
    //     var original = "Loo";
    //     var @new = "lot";
    //     var rack = "tar";
    //
    //     Assert.IsFalse(wordChecker.CanFormNewWord(original, @new, rack),
    //         "Original word double letter deficit failed. Expected false: Loo -> lot.");
    // }
    //
    // [Test] // Original word is Loo, new word is lotto, rack word is tart
    // public void TestDoubleLettersInNewWordAndInRackWord_ResultTrue()
    // {
    //     // Original needs two 'o'. New has two 'o'. (Passes Step 1)
    //     // New word has two 't' not required by original. Remaining: 't', 't'. 
    //     // Rack: 't', 'a', 'r', 't'. Rack has two 't's. Passes.
    //     var original = "Loo";
    //     var @new = "lotto";
    //     var rack = "tart";
    //
    //     Assert.IsTrue(wordChecker.CanFormNewWord(original, @new, rack),
    //         "Double letters in rack test failed. Expected true: Loo -> lotto (remaining 'tt') in tart.");
    // }
    //
    // [Test]
    // // Original Word is Loo, new word is lotto, rack word is tar.
    // public void TestDoubleLettersInNewWordNotEnoughInRackWord_ResultFalse()
    // {
    //     // Original needs two 'o'. New has two 'o'. (Passes Step 1)
    //     // New word has two 't' not required by original. Remaining: 't', 't'.
    //     // Rack: 't', 'a', 'r'. Rack has only one 't'. (Fails Step 3)
    //     var original = "Loo";
    //     var @new = "lotto";
    //     var rack = "tar";
    //
    //     Assert.IsFalse(wordChecker.CanFormNewWord(original, @new, rack),
    //         "Rack word double letter deficit failed. Expected false: Loo -> lotto (remaining 'tt') in tar.");
    // }
    //
    // [Test] // Additional Test for Case Insensitivity and complex Remaining
    // public void TestCaseInsensitivityAndComplexRemaining_ResultTrue()
    // {
    //     // Original: 'b', 'a', 'n', 'a', 'n', 'a' (3xA, 2xN, 1xB)
    //     // New: 'B', 'a', 'n', 'd', 'a', 'n', 'a' (3xA, 2xN, 1xB, 1xD)
    //     // Remaining: 'd'. Rack: 'D', 'o', 'g'. Passes.
    //     var original = "bAnaNa";
    //     var @new = "BanDANA";
    //     var rack = "Dog";
    //
    //     Assert.IsTrue(wordChecker.CanFormNewWord(original, @new, rack),
    //         "Case insensitivity test failed. Expected true: bAnaNa -> BanDANA (remaining 'd') in Dog.");
    // }
}