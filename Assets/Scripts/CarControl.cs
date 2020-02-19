using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControl : MonoBehaviour
{ // Map.
    private MapCreater myMap;


    private Vector3 screenPoint;
    private Vector3 offset; 
    private Vector3 newPosition;

    private float leftOffset = -0.5f;
    private float rightOffset = 0.5f;
    private float centerOffset = 0;
    //private float speed = 20.0f;
    


    void OnMouseDown()
    {

        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;


        float diffx = curPosition.x  - transform.position.x;
        float diffy = curPosition.y - transform.position.y;

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
        // the position of left side of the car
        float leftx = transform.position.x + dx + leftOffset;
        // the position of right side of the car
        float rightx = transform.position.x + dx + rightOffset;
        float nx = transform.position.x + dx + centerOffset;
        int ny = (int)transform.position.y + dy;
        Debug.Log(leftx.ToString() + " " + nx.ToString() + " " + rightx.ToString());
        // detect whether there is a wall in the front part of the car or back part of the car.
        if (isWall((int)leftx, ny) || isWall((int)rightx, ny)) return;

        // detect Exit
        if (isExit((int)leftx, ny)){
          gameObject.SetActive(false);
        }

        transform.position = new Vector3(nx, ny);
        /*// Move our position a step closer to the target.
        float step = speed * Time.deltaTime; // calculate distance to move
        // Move player to next position.
        transform.position = transform.position = Vector3.MoveTowards(transform.position, new Vector3(nx, ny), step);
        Debug.Log(transform.position.x.ToString() +" " + transform.position.y.ToString() + " " + nx.ToString() +" " + ny.ToString());*/
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
