using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class CarControl : MonoBehaviour
{ // Map.

    [SerializeField] Text tipstext;
    private MapCreater myMap;
    public static float tips = 0;

    private Vector3 screenPoint;
    private Vector3 offset; 
    private Vector3 newPosition;

    private float leftOffset = -0.5f;
    private float rightOffset = 0.5f;
    private float centerOffset = 0;
    //private float speed = 20.0f;



    public float timeToRemoveTheCar;
    public float timeToGivePenalty;
    
    private Vector3 oldLocation;

    /* car timer */
    float currentTime = 0f;
    float startTime = 0f;
    public Text countdownText;
    private GeneratingCars mySet;


    void Start()
    {
        timeToGivePenalty = gameObject.GetComponent<CarTimer>().timeToGivePenalty;
        timeToRemoveTheCar = gameObject.GetComponent<CarTimer>().timeToRemoveTheCar;

    }


    // Update is called once per frame
    void Update()
    {
        currentTime = gameObject.GetComponent<CarTimer>().currentTime;
        tipstext.text = "Tips: " + tips.ToString(); 
    }
    /* car timer */

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
        //Debug.Log(leftx.ToString() + " " + nx.ToString() + " " + rightx.ToString());
        // detect whether there is a wall in the front part of the car or back part of the car.
        if (isWall((int)leftx, ny) || isWall((int)rightx, ny)) return;


        if (isEntranceBarrier((int)rightx, ny) || isEntranceBarrier((int)leftx, ny))
        {
            Debug.Log("Too many cars at the entrance.");
        }

        //if either the left side position or right side position is already a car, return
        if(isCar((int)leftx,ny) && isCar((int)rightx,ny)){
            return;
        }
        //if not move car
        else if(isCar((int)leftx,ny) || isCar((int)rightx,ny)){
            //can move if moving to left or right 
            if(dx>=1 || dx<=-1){
                oldLocation = transform.position;
                //remove old loc
                int newLeftX = (int)(oldLocation.x+leftOffset);
                int newRightX = (int)(oldLocation.x+rightOffset);
                mySet.set.Remove(mySet.TwoDToOneD(newLeftX,(int)oldLocation.y));
                mySet.set.Remove(mySet.TwoDToOneD(newRightX,(int)oldLocation.y));
                //add new loc
                mySet.set.Add(mySet.TwoDToOneD((int)leftx,ny));
                mySet.set.Add(mySet.TwoDToOneD((int)rightx,ny));
                transform.position = new Vector3(nx, ny);
            }
            else{
                return;
            }
            
        }
        
        // detect Exit
        if (isExit((int)leftx, ny)){         
          gameObject.SetActive(false);
          mySet.set.Remove(mySet.TwoDToOneD((int)leftx,ny));
          mySet.set.Remove(mySet.TwoDToOneD((int)rightx,ny));
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
        mySet = FindObjectOfType<GeneratingCars>();
    }

    bool isCar(int x, int y){
        return mySet.set.Contains(myMap.TwoDToOneD(x,y));
    }


    bool isWall(int x, int y)
    {
        return myMap.getWallPosSet().Contains(myMap.TwoDToOneD(x, y));
    }

    bool isExit(int x, int y)
    {
        Vector3 exitPos = myMap.getExitPos();
        if (exitPos[0] == x && exitPos[1] == y ){
            if (currentTime <= timeToRemoveTheCar && currentTime >= timeToGivePenalty)
            {
                tips += 10;

            } else
            {
                tips -= 5;
            }

            Debug.Log(exitPos); 
           return true;
        }
        return false;
    }

    bool isEntranceBarrier(int x, int y)
    {
        Vector3 entranceBarrierPos = myMap.getEntranceBarrierPos();
        if (entranceBarrierPos[0] == x && entranceBarrierPos[1] == y)
        {
            Debug.Log(entranceBarrierPos);
            return true;
        }
        return false;
    }
}
