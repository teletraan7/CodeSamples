using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Manager : MonoBehaviour
{
    GameObject[] pauseObjects;

    void Start()
    {
        Time.timeScale = 1;
        pauseObjects = GameObject.FindGameObjectsWithTag("ShowOnPause");
        hidePaused();
        
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
                showPaused();
            }

            else if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
                hidePaused();
            }
        }
    }

    public void pauseControl()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            showPaused();
        }

        else if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            hidePaused();
        }
    }


            //shows objects with showonpause tag
            public void showPaused()
            {
              foreach(GameObject g in pauseObjects)
                {
                    g.SetActive(true);
                }
            }

            //hides objects with showonpause tag
            public void hidePaused()
            {
                foreach (GameObject g in pauseObjects)
                {
                    g.SetActive(false);
               }
            }

    public void LoadLevel(string level) { 
        SceneManager.LoadScene(level);
        }

    public void exitGame()
    {
        Application.Quit();
    }

}
