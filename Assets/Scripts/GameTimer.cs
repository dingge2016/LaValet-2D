using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    public float currentTime = 0f;
    [SerializeField] float startTime;
    [SerializeField] Text countdownText;
    public GameObject gameOverUI;

    // Start is called before the first frame update
    void Start()
    {
        currentTime = startTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOverUI != null && gameOverUI.activeSelf)
        {
            return;
        }
        currentTime -= 1 * Time.deltaTime;     // Time.deltaTime to make time be updated by second not by frame

        int seconds = (int)(currentTime % 60);
        int minutes = (int)(currentTime / 60);

        string timerString = string.Format("{0:0}:{1:00}", minutes, seconds);

        countdownText.text = timerString;

        if (currentTime <= 0)
        {
            currentTime = 0;
        }

        if (currentTime == 0)
        {
            SceneManager.LoadScene(4);
        }

    }
}
