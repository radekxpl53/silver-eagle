using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class SmokeTest
{
    [UnityTest]
    public IEnumerator GameManagerSceneLoadsWithoutErrors()
    {
        yield return SceneManager.LoadSceneAsync("GameManager", LoadSceneMode.Single);
        yield return null;
        yield return null;

        Assert.IsNotNull(GameManager.Instance);
        Assert.IsNotNull(ChunkManager.Instance);
    }
}
