using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public GameObject gameOverUI;

    private void Start()
    {
        gameOverUI.SetActive(false);
    }
    void Update()
    {
        Text carText = GameObject.Find("Canvas/EndGameText").GetComponent<Text>();
        string tooManyCars = "Too many cars at the entrance. Game is over!";
        if (string.Equals(carText.text, tooManyCars))
        {
            float cur = PlayerPrefs.GetFloat("totalTips", 0);
            PlayerPrefs.SetFloat("totalTips", cur + CarControl.tips);
            gameOverUI.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
