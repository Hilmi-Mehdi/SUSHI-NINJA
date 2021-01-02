using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
            StartCoroutine(LoadNext());
    }
    IEnumerator LoadNext()
    {
        Time.timeScale = 0.3f;
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1f;
        var currenSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currenSceneIndex + 1);
    }
}
