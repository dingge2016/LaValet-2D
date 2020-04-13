using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SceneLoader : MonoBehaviour
{
    public static int levelNumber = 0;
    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;   // GetActiveScene() get the scene we are currently on
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public void LoadCurrentScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void LoadStartScene()
    {
        SceneManager.LoadScene(0);
    }



    public void LoadGameScreen()
    {
        string levelName = EventSystem.current.currentSelectedGameObject.name;
  
        string levelOne = "Level 1 Button";
        string levelTwo = "Level 2 Button";
        string levelThree = "Level 3 Button";
        string levelFour = "Level 4 Button";


        if (string.Equals(levelName, levelOne))
        {
            levelNumber = 1;
        }
        else if (string.Equals(levelName, levelTwo))
        {
            levelNumber = 2;
        }
        else if (string.Equals(levelName, levelThree))
        {
            levelNumber = 3;
        }
        else if (string.Equals(levelName, levelFour))
        {
            levelNumber = 4;
        }


        if (levelNumber == 1)
        {
            SceneManager.LoadScene(2);
        }
        else if (levelNumber == 2) {
            SceneManager.LoadScene(3);
        }
        else if (levelNumber == 3)
        {
            SceneManager.LoadScene(4);
        }
        else if (levelNumber == 4)
        {
            SceneManager.LoadScene(5);
        }
    }

    public void PlayAgain()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex - 1);
    }

    public void QuitGame() {
        Application.Quit();
    }

    public int getLevelIndex() {
        return SceneManager.GetActiveScene().buildIndex-1;
    }
}
