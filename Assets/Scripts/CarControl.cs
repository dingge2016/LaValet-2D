using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControl : MonoBehaviour
{ // Map.
    private MapCreater myMap;


    private Vector3 screenPoint;
    private Vector3 offset;


    void OnMouseDown()
    {

        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;

        int nx = (int)curPosition.x;
        int ny = (int)curPosition.y;

        // detect wall
        if (isWall(nx, ny)) return;

  
        // Move player to next position.
        transform.position = new Vector3(nx, ny);

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
