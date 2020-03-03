using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarTimer : MonoBehaviour
{ 

    public float currentTime = 0f;
    float startTime = 0f;
    [SerializeField] Text countdownText;

    public float timeToGivePenalty;
    public float timeToRemoveTheCar;


    void Start()
    {
        startTime = Random.Range(5.0f, 15.0f);
        currentTime = startTime;
    }



    //Update is called once per frame
    void Update()
    {
        currentTime -= 1 * Time.deltaTime;     // Time.deltaTime to make time be updated by second not by frame

        int seconds = (int)(currentTime % 60);
        int minutes = (int)(currentTime / 60);

        string timerString = string.Format("{0:0}:{1:00}", minutes, seconds);

        if (currentTime <= 6)
        {
            countdownText.color = Color.red;
        }
        else
        {
            countdownText.color = Color.white;
        }

        if (currentTime <= 0)
        {
            currentTime = 0;
        }

        Vector3 carTimerPos = Camera.main.WorldToScreenPoint(this.transform.position);
        countdownText.transform.position = carTimerPos;
        if (currentTime > timeToRemoveTheCar)
            countdownText.text = "";
        else
            countdownText.text = timerString;
    }
}
