using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarTimer : MonoBehaviour
{

    public float currentTime;
    float startTime = 0f;
    public Text countdownText;

    private Renderer rend;
    private float removeCarTime;
    public float minTime, maxTime;

    void Start()
    {
        if (currentTime == 0f){
          startTime = Random.Range(minTime, maxTime);
          currentTime = startTime;
        }

        //Get the renderer of the object so we can access the color
        removeCarTime = gameObject.GetComponent<CarControl>().timeToRemoveTheCar;
        rend = gameObject.transform.Find("CarPicture").GetComponent<Renderer>();   // color
        rend.material.color = Color.green;
    }



    //Update is called once per frame
    void Update()
    {
        currentTime -= 1 * Time.deltaTime;     // Time.deltaTime to make time be updated by second not by frame

        int seconds = (int)(currentTime % 60);
        int minutes = (int)(currentTime / 60);

        string timerString = string.Format("{0:0}:{1:00}", minutes, seconds);

        // Car color change

        if (currentTime > removeCarTime)
        {
            rend.material.color = Color.blue;
        }
        else if (currentTime <= 6)
        {
            rend.material.color = Color.red;
        }
        else
        {
            rend.material.color = Color.green;
        }

        // Car timer color change
        if (currentTime <= 6)
        {
            countdownText.color = Color.white;
        }
        else
        {
            countdownText.color = Color.white;
        }

        if (currentTime <= 0)
        {
            rend.material.color = Color.black;
            currentTime = 0;
        }

        Vector3 carTimerPos = Camera.main.WorldToScreenPoint(this.transform.position);
        countdownText.transform.position = carTimerPos;
        countdownText.text = timerString;
    }
}
