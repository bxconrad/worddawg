using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class NewTestScript
    {
        // A Test behaves as an ordinary method
        [Test]
        public void NewTestScriptSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        //   [Test]
        // [TestMethod]
        // Original word is lop, new word is lope, rack word is ear
        public void SimpleTest_ResultTrue()
        {
            // Remaining: 'e'. Rack: 'e', 'a', 'r'. Passes.
            var original = "lop";
            var @new = "lope";
            var rack = "ear";
            var gameObject = new GameObject();
            //     gameObject.AddComponent<WordChecker>();
        }
    }
}