using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarControl : MonoBehaviour
{ // Map.
    protected MapCreater myMap;
    private Vector3 offset;
    private Vector3 newPosition;
    GameObject gate;

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

    public bool touchStart = false;
    public Vector3 pointA;
    public Vector3 pointB;
    public string name;

    public Control control;
    public Vector3 originalPos;
    // Update is called once per frame

    void Update()
    {
        currentTime = gameObject.GetComponent<CarTimer>().currentTime;
        if (currentTime <= timeToGivePenalty && !minusTip)
        {
            GameManager.totalTips -= 5;
            minusTip = true;
        }


         //use raycast to select car
        if (Input.GetMouseButtonDown(0)){
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10;

            Vector3 screenPos = Camera.main.ScreenToWorldPoint(mousePos);

            RaycastHit2D hit = Physics2D.Raycast(screenPos,Vector2.zero);

            //click on the car that needs to moved
            //when raycast hits cars
            if(hit)
            {
                //save the name of the clicked car
                name = hit.collider.name;
            }
        }

            //if name of the clicked car is same as the gameobject that this script is attached to, move the car
            if(name==this.gameObject.name){
                //returns true during the frame the mouse is pressed down
                if(Input.GetMouseButtonDown(0)){
                    //obtain original position
                    pointA = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
                    control.circle.transform.position = originalPos;
                }
                //mouse is held down
                if(Input.GetMouseButton(0)){
                    touchStart = true;
                    //obtain final position
                    pointB = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
                }
                else{
                    touchStart = false;
                }
            }

    }

    //updated edit->projectsettings->fixed timestep to 0.1 (originally 0.02), changed sensitivity so car doesn't move too fast
    private void FixedUpdate(){
        if(touchStart){
            //calculate how much car has moved
            Vector3 offset = pointB-pointA;
            movetheSelectedCar(offset);
            //limit movement to 1.0f so that button doesn't go out of joystick
            Vector3 direction = Vector3.ClampMagnitude(offset,1.0f);
            control.circle.transform.position = new Vector3(originalPos.x + direction.x, originalPos.y + direction.y, 0);
        }
        /*
        else{
            control.circle.transform.position = originalPos;
        }
        */
    }
    /* car timer */

    /*
    void OnMouseDown()
    {

        //obtain world position of game object
        //subtract mouse position from game object position
        //offset is how much mouse's position differs from object's position
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));

    }
    */


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
        //float diffx = curPosition.x - transform.position.x;
        //float diffy = curPosition.y - transform.position.y;

        //changed diffx and diffy
        float diffx = curPosition.x;
        //Debug.Log("diffx" + diffx);
        float diffy = curPosition.y;
        //Debug.Log("diffy" + diffy);

        var dxAndDy = flattenDiff(diffx, diffy);

        int dx = dxAndDy.Key;
        int dy = dxAndDy.Value;
        if (dx == 0 && dy == 0)
            return;

        // the position of left side of the car
        float leftx = transform.position.x + dx + leftOffset;
        //Debug.Log("transform.position.x" + transform.position.x);
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
            //Debug.Log("there is a car1");
            return;
        } else if (dx == -1 && isCar((int)leftx, ny)) {
            // Debug.Log("dx == -1", leftx, ny);
            //Debug.Log("there is a car2");
            return;

        }
        else if (dx == 0 && (isCar((int)leftx, ny) || isCar((int)rightx, ny)))
        { // when car move up or move down
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

    /*
    void OnMouseDrag()
    {
        //new position
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        //move the car to mouse's new position
        movetheSelectedCar(curPosition);
    }
    */


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
        transform.position = new Vector3(nx, ny, -2);
        carInLot((int)leftx, (int)rightx, ny);
    }

    // determine if the car park in the special gate lot
    public void carInLot(int carLeftX, int carRightX, int carY)
    {
        // check == 1, half car in gate lot; check == 2, full car in gate lot
        int check = 0;
        string tagText = "";
        string halfCar = "";
        foreach (var lot in myMap.gate_lot_pos)
        {
            if (lot.Key == carLeftX && lot.Value == carY)
            {
                check++;
                halfCar = "leftcar";
            }
            if (lot.Key == carRightX && lot.Value == carY)
            {
                check++;
                halfCar = "rightcar";
            }
            if (check == 2)
            {
                tagText = posToTag(carLeftX) + posToTag(carRightX) + posToTag(carY);
                gate = GameObject.FindWithTag(tagText);
                gate.SetActive(false);
                break;
            }
        }
        if (check == 1 && string.Equals(halfCar, "leftcar"))
        {
            tagText = posToTag(carLeftX - 1) + posToTag(carRightX - 1) + posToTag(carY);
            gate = GameObject.FindWithTag(tagText);
            gate.SetActive(true);
        }
        else if (check == 1 && string.Equals(halfCar, "rightcar"))
        {
            tagText = posToTag(carLeftX + 1) + posToTag(carRightX + 1) + posToTag(carY);
            gate = GameObject.FindWithTag(tagText);
            gate.SetActive(true);
        }
    }

    // conversion for matching gate tag
    public string posToTag(int pos)
    {
        if (pos >= 0)
        {
            return "+" + pos;
        }
        return "" + pos;
    }

    private void Awake()
    {
        myMap = FindObjectOfType<MapCreater>();
        control = FindObjectOfType<Control>();
        //original pos of joystick button
        originalPos = new Vector3(-7.0f,0.5f,0f);
        //Debug.Log("pos of circle when awake " + originalPos);
    }

    protected bool isCar(int x, int y){
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
            if (GameObject.Find("GameManager").GetComponent<GameManager>().duringDoubleTipsTime)
                GameManager.totalTips += 20;
            else GameManager.totalTips += 10;

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
