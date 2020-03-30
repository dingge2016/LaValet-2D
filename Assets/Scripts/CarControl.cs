using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarControl : MonoBehaviour
{ // Map.
    protected MapCreater myMap;
    private Vector3 offset;
    private Vector3 newPosition;

    private float leftOffset = -0.5f;
    private float rightOffset = 0.5f;
    protected float centerOffset = 0;
    //private float speed = 20.0f;

    public float timeToRemoveTheCar;
    public float timeToGivePenalty;
    private Vector3 oldLocation;

    /* car timer */
    float currentTime = 0f;
    public Text countdownText;
    bool minusTip;

    // Update is called once per frame
    void Update()
    {
        currentTime = gameObject.GetComponent<CarTimer>().currentTime;
        if (currentTime <= timeToGivePenalty && !minusTip)
        {
            GameManager.totalTips -= 5;
            minusTip = true;
        }
    }
    /* car timer */

    void OnMouseDown()
    {

        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
    }


    protected KeyValuePair<int, int> flattenDiff(float diffx, float diffy)
    {
        int dx = 0;
        int dy = 0;

        if (diffx >= 1)
            dx = 1;
        else if (diffx <= -1)
            dx = -1;

        else if (diffy >= 1)
            dy = 1;
        else if (diffy <= -1)
            dy = -1;
        return new KeyValuePair<int, int>(dx, dy);
    }

    public virtual void movetheSelectedCar(Vector3 curPosition)
    {
        float diffx = curPosition.x - transform.position.x;
        float diffy = curPosition.y - transform.position.y;

        var dxAndDy = flattenDiff(diffx, diffy);

        int dx = dxAndDy.Key;
        int dy = dxAndDy.Value;
        if (dx == 0 && dy == 0)
            return;

        // the position of left side of the car
        float leftx = transform.position.x + dx + leftOffset;
        // the position of right side of the car
        float rightx = transform.position.x + dx + rightOffset;
        float nx = transform.position.x + dx + centerOffset;
        int ny = (int)transform.position.y + dy;
        //Debug.Log(leftx.ToString() + " " + nx.ToString() + " " + rightx.ToString());
        // in Enter, Can't Go Back
        Vector3 entranceBarrierPos = myMap.getEntranceBarrierPos();
        Vector3 startPos = new Vector3(entranceBarrierPos[0]+ leftOffset, entranceBarrierPos[1]);
        if (new Vector3(transform.position.x, transform.position.y) == startPos && dx == -1){
          return;
        }

        // detect whether there is a wall in the front part of the car or back part of the car.
        if (isWall((int)leftx, ny) || isWall((int)rightx, ny)) {
          return;
        }

        if (dx == 1 && isCar((int)rightx, ny)) {
            // Debug.Log("dx == 1 :" , rightx, ny);
            return;
        } else if (dx == -1 && isCar((int)leftx, ny)) {
            // Debug.Log("dx == -1", leftx, ny);
            return;

        }
        else if (dx == 0 && (isCar((int)leftx, ny) || isCar((int)rightx, ny)))
        { // when car move up or move down
          // Debug.Log('dx')
            return;
        }

        //Debug.Log("Ok to remove");

        if (isExit((int)leftx + 1, ny) && currentTime > timeToRemoveTheCar)
        {
            return;
        }

        //transform.position = new Vector3(nx, ny);
        moveCar(nx, ny, (int)rightx, (int)leftx);

        if (isExit((int)leftx, ny))
        {
            updateTips();
            myMap.removeCars((int)leftx,ny);
            myMap.removeCars((int)rightx,ny);
            Destroy(gameObject);
        }
        /*// Move our position a step closer to the target.
        float step = speed * Time.deltaTime; // calculate distance to move
        // Move player to next position.
        transform.position = transform.position = Vector3.MoveTowards(transform.position, new Vector3(nx, ny), step);
        Debug.Log(transform.position.x.ToString() +" " + transform.position.y.ToString() + " " + nx.ToString() +" " + ny.ToString());*/
    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        movetheSelectedCar(curPosition);

    }


    void moveCar(float nx, int ny, int rightx, int leftx){
        oldLocation = transform.position;
        //remove old loc
        int newLeftX = (int)(oldLocation.x+leftOffset);
        int newRightX = (int)(oldLocation.x+rightOffset);
        myMap.removeCars(newLeftX,(int)oldLocation.y);
        myMap.removeCars(newRightX,(int)oldLocation.y);

        //add new loc
        myMap.addCars((int)leftx,ny);
        myMap.addCars((int)rightx,ny);
        transform.position = new Vector3(nx, ny);
    }



    private void Awake()
    {
        myMap = FindObjectOfType<MapCreater>();
    }

    protected bool isCar(int x, int y){
        if (myMap.getCarsPosSet().Contains(myMap.TwoDToOneD(x,y))){
          myMap.printCarPos();
        }
        return myMap.getCarsPosSet().Contains(myMap.TwoDToOneD(x,y));
    }


    protected bool isWall(int x, int y)
    {
        return myMap.getWallPosSet().Contains(myMap.TwoDToOneD(x, y));
    }

    void updateTips()
    {
        if (currentTime > timeToGivePenalty)
        {
            GameManager.totalTips += 10;

        }

    }
    protected bool isExit(int x, int y)
    {
        Vector3 exitPos = myMap.getExitPos();
        if (exitPos[0] == x && exitPos[1] == y ){

           return true;
        }
        return false;
    }


}
