using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarControl : MonoBehaviour
{ // Map.
    protected MapCreater myMap;
    protected GameManager myGameManager;
    private Vector3 offset;
    private Vector3 newPosition;
    public List<string> allGates = new List<string>();
    private GameObject gateOne;
    private GameObject gateTwo;

    private float leftOffset = -0.5f;
    private float rightOffset = 0.5f;
    protected float centerOffset = 0;
    //private float speed = 20.0f;

    public float timeToRemoveTheCar;
    public float timeToGivePenalty;
    private Vector3 oldLocation;
    private int beltMoveTime = 0;
    /* car timer */
    float currentTime = 0f;
    public Text countdownText;
    bool minusTip;
     
    public bool bombClicked;

    void Start()
    {
        allGates.Add("+0+1+3");
        allGates.Add("+0+1-3");
        gateOne = GameObject.Find("GateOne");
        gateTwo = GameObject.Find("GateTwo");
    }


    protected int flattenDiff(float diff)
    {
        int flattenedDiff = 0;

        if (diff >= 0.7)
            flattenedDiff = 1;
        else if (diff <= -0.7)
            flattenedDiff = -1;

        return flattenedDiff;
    }


    public void detectedMouseandMovetheSeletctedCar()
    {
      //continuly move if in belt
      if ( myMap.isbeltOn() && isBelt() ){
        if (beltMoveTime % 15 == 0){
          determineMove(+1,0);
        }
        beltMoveTime += 1;
      }// Otherwise use mouse to drag
      if (Input.GetMouseButton(0) && myGameManager.getSelectedCar() != null && myGameManager.getSelectedCar() == gameObject)
      {
          Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
          Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
          //move the car to mouse's new position
          movetheSelectedCarOrTrain(curPosition);
      }
    }
    
    //after clicking on the bomb sprite, the car selected gets destroyed
    public void detectBombAndCar(){
        //if bomb can be used and has been selected
        if(myGameManager.bombActive()==true && bombClicked == true){
        Vector3 oldCarLocation = myGameManager.getSelectedCar().transform.position;
        //destroy the gameobject
        Destroy(myGameManager.getSelectedCar());
        //remove car position from hashset
        int newLeftXLoc = (int)(oldCarLocation.x+leftOffset);
        int newRightXLoc = (int)(oldCarLocation.x+rightOffset);
        myMap.removeCars(newLeftXLoc,(int)oldCarLocation.y);
        myMap.removeCars(newRightXLoc,(int)oldCarLocation.y);
        //remove driver
        myGameManager.driver.GetComponent<Renderer>().enabled = false;
        //turn off bomb
        myGameManager.bomb.SetActive(false);
        }
    }

    void Update()
    {
        currentTime = gameObject.GetComponent<CarTimer>().currentTime;
        if (currentTime <= timeToGivePenalty && !minusTip)
        {
            GameManager.totalTips -= 5;
            minusTip = true;
        }

        //check if bomb has been selected 
        if (Input.GetMouseButtonDown(0)){
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10;
 
            Vector3 screenPos = Camera.main.ScreenToWorldPoint(mousePos);
 
            RaycastHit2D hit = Physics2D.Raycast(screenPos,Vector2.zero);
            
            if(hit)
            {
                //if item selected is bomb
                if(hit.collider.name == "bomb_circle"){
                    //set bomb to active 
                    bombClicked = true;

                }
            }
        }

        detectedMouseandMovetheSeletctedCar();
        detectBombAndCar();
        


    }

    void OnMouseDown()
    {

        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        //if(gameObject.tag == "car"){
        myGameManager.setSelectedCar(gameObject);
        //}
        

    }

    protected virtual void movetheSelectedCarOrTrain(Vector3 curPosition)
    {
        float diffx = curPosition.x - transform.position.x;
        float diffy = curPosition.y - transform.position.y;

        int dx = flattenDiff(diffx);
        int dy = flattenDiff(diffy);

        if (dx == 0 && dy == 0)
            return;

        if (Mathf.Abs(diffx) >= Mathf.Abs(diffy))
        {
            if (!determineMove(dx, 0))
            {
                determineMove(0, dy);
            }
        }
        else
        {
            if (!determineMove(0, dy))
            {
                determineMove(dx, 0);
            }
        }

    }

    private bool determineMove(int dx, int dy){
      if (dx == 0 && dy == 0)
          return false;

      // the position of left side of the car
      float leftx = transform.position.x + dx + leftOffset;
      float rightx = transform.position.x + dx + rightOffset;

      float nx = transform.position.x + dx + centerOffset;
      int ny = (int)transform.position.y + dy;
      if (isExit((int)leftx + 1, ny) && currentTime > timeToRemoveTheCar)
      {
          return false;
      }
      // in Enter, Can't Go Back
      if (isEntry((int)(transform.position.x+leftOffset), (int)(transform.position.x+rightOffset), (int)transform.position.y) && dx == -1){
        return false;
      }

      // detect whether there is a wall in the front part of the car or back part of the car.
      if (isWall((int)leftx, ny) || isWall((int)rightx, ny)) {
        return false;
      }

      if (dx == 1 && isCar((int)rightx, ny)) {
          return false;
      } else if (dx == -1 && isCar((int)leftx, ny)) {
          return false;

      } else if (dx == 0 && (isCar((int)leftx, ny) || isCar((int)rightx, ny)))
      { // when car move up or move down
          return false;
      }
      if (isBelt()){
        moveCarOnBelt(nx, ny, (int)rightx, (int)leftx);
      } else {
        moveCar(nx, ny, (int)rightx, (int)leftx);
      }

      if (isExit((int)rightx, ny))
      {
          updateTips();
          myMap.removeCars((int)leftx,ny);
          myMap.removeCars((int)rightx,ny);
          myGameManager.setSelectedCar(null);
          Destroy(gameObject);

      }

      return true;
    }

    void moveCarOnBelt(float nx, int ny, int rightx, int leftx){
        oldLocation = transform.position;
        if (! (oldLocation.y == ny) ){
          //Vertical Move => Could be done]
          moveCar(nx, ny, rightx, leftx);
        } else {
          //Horizontal Move => Couldn't be done
          if ( myMap.isbeltOn() ){
            moveCar(nx, ny, rightx, leftx);
          } else {
            return;
          }
        }
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
        transform.position = new Vector3(nx, ny, -2);
        carInLot((int)leftx, (int)rightx, ny, newLeftX, newRightX, (int)oldLocation.y);
    }

    // determine if the car park in the special gate lot
    public void carInLot(int carLeftX, int carRightX, int carY, int oldCarLeftX, int oldCarRightX, int oldCartY)
    {
        // check if new car position is in gate lot
        // checkNew == 1, half car in gate lot; checkNew == 2, full car in gate lot
        int checkNew = 0;
        int checkOld = 0;
        string tagText = "";
        string halfCar = "";
        foreach (var lot in myMap.gate_lot_pos)
        {
            if (lot.Key == carLeftX && lot.Value == carY)
            {
                checkNew++;
                halfCar = "leftcar";
            }
            if (lot.Key == carRightX && lot.Value == carY)
            {
                checkNew++;
                halfCar = "rightcar";
            }
            if (checkNew == 2)
            {
                tagText = posToTag(carLeftX) + posToTag(carRightX) + posToTag(carY);
                openGate(tagText);
            }
        }
        if (checkNew == 1 && string.Equals(halfCar, "leftcar"))
        {
            tagText = posToTag(carLeftX - 1) + posToTag(carRightX - 1) + posToTag(carY);
            closeGate(tagText);
        }
        else if (checkNew == 1 && string.Equals(halfCar, "rightcar"))
        {
            tagText = posToTag(carLeftX + 1) + posToTag(carRightX + 1) + posToTag(carY);
            closeGate(tagText);
        }

        // check if old car position is in gate lot
        // checkOld == 2 the car is in a gate lot
        foreach (var lot in myMap.gate_lot_pos)
        {
            if ((lot.Key == oldCarLeftX && lot.Value == oldCartY) || (lot.Key == oldCarRightX && lot.Value == oldCartY))
            {
                checkOld++;
            }
            if (checkOld == 2)
            {
                gateOne.SetActive(true);
                gateTwo.SetActive(true);
            }
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

    // open a gate
    public void openGate(string tagText)
    {
        for (int i = 0; i < allGates.Count; i++)
        {
            if (string.Equals(tagText, allGates[i]))
            {
                if (i == 0) gateOne.SetActive(false);
                if (i == 1) gateTwo.SetActive(false);
            }
        }
    }

    // close a gate
    public void closeGate(string tagText)
    {
        for (int i = 0; i < allGates.Count; i++)
        {
            if (string.Equals(tagText, allGates[i]))
            {
                if (i == 0) gateOne.SetActive(true);
                if (i == 1) gateTwo.SetActive(true);
            }
        }
    }

    private void Awake()
    {
        myMap = FindObjectOfType<MapCreater>();
        myGameManager = FindObjectOfType<GameManager>(); 
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
            if (myGameManager.isDuringDoubleTipsTime())
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

    protected bool isEntry(int leftx, int rightx, int y)
    {
        // First Entry
        Vector3 entryPos = myMap.getEntranceBarrierPos();
        if ((entryPos[0] == leftx && entryPos[1] == y) || (entryPos[0] == rightx && entryPos[1] == y))
        {
            return true;
        }
        // Second Entry
        List<KeyValuePair<int, int>> entryPos2 = myMap.multi_ent_pos_set;
        if ((entryPos2[0].Key == leftx && entryPos2[0].Value == y) || (entryPos2[0].Key == rightx && entryPos2[0].Value == y))
        {
            return true;
        }
        return false;
    }

    protected bool isBelt()
    {
        Vector3 curPos = transform.position;
        int leftPos = (int)(curPos.x+leftOffset);
        int rightPos = (int)(curPos.x+rightOffset);
        return myMap.getBeltPosSet().Contains(myMap.TwoDToOneD(leftPos, (int)curPos.y)) || myMap.getBeltPosSet().Contains(myMap.TwoDToOneD(rightPos, (int)curPos.y));
    }

}
