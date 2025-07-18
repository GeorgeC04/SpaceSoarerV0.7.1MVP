using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void PlayerTestSimplePasses()
    {
        
        Player newplayer = new Player();
        Assert.AreEqual(10, newplayer.bulletSpeed);
        Assert.IsNotNull(newplayer.bulletSpeed);
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator PlayerTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
