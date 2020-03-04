using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public GameObject gameOverUI;
    public GameObject gameovertext;

    private void Start()
    {
        gameOverUI.SetActive(false);
    }
    void Update()
    {
        GameTimer timeLeft = gameovertext.GetComponent<GameTimer>();
        if (timeLeft != null && timeLeft.currentTime <= 10f)
        {
            float cur = PlayerPrefs.GetFloat("totalTips", 0);
            PlayerPrefs.SetFloat("totalTips", cur + CarControl.tips);
            gameOverUI.SetActive(true);
        }
    }
}
