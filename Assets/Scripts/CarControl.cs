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


        int diffx = (int)curPosition.x  - (int)transform.position.x;
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

        // detect wall
        if (isWall(nx, ny)) return;

        // detect Exit
        if (isExit(nx, ny)){
          gameObject.SetActive(false);
        }

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

    bool isExit(int x, int y)
    {
        Vector3 exitPos = myMap.getExitPos();
        if (exitPos[0] == x && exitPos[1] == y ){
          Debug.Log(exitPos);
          return true;
        }
        return false;
    }
}
