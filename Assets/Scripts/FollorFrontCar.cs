﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollorFrontCar : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject FrontCarPart;
    private Vector3 screenPoint;
    private Vector3 offset;
    private MapCreater myMap;
    private Vector3 newPosition;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        newPosition.x = FrontCarPart.transform.position.x - 1;
        newPosition.y = FrontCarPart.transform.position.y;

        transform.position = newPosition;

     /*   Debug.Log(newPosition);*/
    }

    void OnMouseDown()
    {

        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;


        int diffx = (int)curPosition.x - (int)transform.position.x;
        int diffy = (int)curPosition.y - (int)transform.position.y;

        int dx = 0;
        int dy = 0;

        if (diffx >= 1)
            dx = 1;
        else if (diffx <= -1)
            dx = -1;

        if (diffy >= 1)
            dy = 1;
        else if (diffy <= -1)
            dy = -1;

        int nx = (int)transform.position.x + dx;
        int ny = (int)transform.position.y + dy;

        // detect whether there is a wall in the front part of the car or back part of the car.
        if (isWall(nx, ny) || isWall(nx + 1, ny)) return;


        // Move player to next position.
        transform.position = new Vector3(nx, ny);
        Debug.Log(nx);
        Debug.Log("carPosition: " + nx.ToString() + "," + ny.ToString());

    }



    private void Awake()
    {
        myMap = FindObjectOfType<MapCreater>();
    }


    bool isWall(int x, int y)
    {
        return myMap.getWallPosSet().Contains(myMap.TwoDToOneD(x, y));
    }
}