using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    public GameObject canvas;
    public GameObject levelcanvas;
    public void Rest()
    {
        var currenSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currenSceneIndex);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void StartNow()
    {
        SceneManager.LoadScene(1);
    }
    public void Quit()
    {
        Application.Quit();
    }
    private void Start()
    {
        Cursor.visible = true;
    }
    private void Update()
    {
        
    }
    public void Levelselect()
    {
        canvas.SetActive(false);
        levelcanvas.SetActive(true);
    }

    public void lvl1()
    {
        SceneManager.LoadScene(2);
    }
    public void lvl2()
    {
        SceneManager.LoadScene(3);
    }
    public void lvl3()
    {
        SceneManager.LoadScene(4);
    }
    public void tuto()
    {
        SceneManager.LoadScene(1);
    }
}
