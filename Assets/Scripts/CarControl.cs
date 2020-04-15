using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarControl : MonoBehaviour
{ // Map.
    protected MapCreater myMap;
    protected GameManager myGameManager;
    protected tipPosition myTipPosition;
    private Vector3 offset;
    private Vector3 newPosition;
    public List<string> allGates = new List<string>();

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

    private bool isCoroutineStarted = false;

    public GameObject tip1;

    void Start()
    {
        if (myMap.isLevelThree())
        {
            allGates.Add("+0+1+3");   // allGates use special lot position to represent gate location e.g. x is from 0 to 1, and y is 3
            allGates.Add("+0+1-3");
        }
        else if (myMap.isLevelFour())
        {
            allGates.Add("+0+1+1");   // Gate One (Level Four)
            allGates.Add("+7+8+3");   // Gate Two (Level Four)
            allGates.Add("+0+1-3");   // Gate Three (Level Four)
            allGates.Add("+7+8-1");   // Gate Four (Level Four)
        }
        else if (myMap.isLevelFive())
        {
            allGates.Add("+5+6+0");   // Gate One (Level Five)
            allGates.Add("+5+6-2");   // Gate Two (Level Five)
        }
        else if (myMap.isLevelSix())
        {
            allGates.Add("+4+5+3");   // Gate One (Level Six)
            allGates.Add("-2-1+1");   // Gate Two (Level Six)
            allGates.Add("+5+6+1");   // Gate Three (Level Six)
            allGates.Add("+5+6-1");   // Gate Four (Level Six)
        }
        else if (myMap.isLevelSeven())
        {
            allGates.Add("-2-1+4");   // Gate One (Level Seven)
            allGates.Add("+3+4+1");   // Gate Two (Level Seven)
            allGates.Add("+3+4+0");   // Gate Three (Level Seven)
            allGates.Add("-2-1-3");   // Gate Four (Level Seven)
        }
        tip1 = GameObject.FindWithTag("tip1");
        tip1.SetActive(false);

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

    //once car turns black, decrease the amount of tips by 1 every second
    private IEnumerator DeductTipEverySecond()
     {
        while(isCoroutineStarted){
            GameManager.totalTips -=1;
            yield return new WaitForSeconds(2f);
        }

     }

     public void showTip1(){
       if (tip1){
         tip1.transform.position = new Vector3(myTipPosition.tipPos.x,myTipPosition.tipPos.y+80f,0);
         tip1.SetActive(true);
       }
        //shows UI for two seconds
        Invoke("HideTip1",2f);
    }

    public void HideTip1(){
      if (tip1){
        tip1.SetActive(false);
      }
    }


    void Update()
    {
        if (myGameManager.isFinishedGame())
        {
            if (tip1){
              tip1.SetActive(false);
            }
            return;

        }
        currentTime = gameObject.GetComponent<CarTimer>().currentTime;
        if (currentTime <= timeToGivePenalty && !minusTip)
        {
            //GameManager.totalTips -= 5;
            minusTip = true;
            //tip gets deducted by 1 every second
            isCoroutineStarted = true;
            StartCoroutine(DeductTipEverySecond());
            showTip1();
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

          myGameManager.showBlueCarText();
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


        // Only check the levels that have gates (Level 3 ~ Level 7)
        if (myMap.isLevelThree() || myMap.isLevelFour() || myMap.isLevelFive() || myMap.isLevelSix() || myMap.isLevelSeven())
        {
            // detect whether there is a gate in the front part of the car or back part of the car.
            if (isGate((int)leftx, ny) || isGate((int)rightx, ny))
            {
                for (int i = 0; i < myMap.gate_pos.Count; i++)
                {
                    if ((myMap.gate_pos[i].Key == (int)leftx || myMap.gate_pos[i].Key == (int)rightx) && (myMap.gate_pos[i].Value == ny))
                    {
                        if (myMap.isLevelThree())
                        {
                            if (i < 2 && myMap.gateOne.activeInHierarchy)
                            {
                                return false;
                            }
                            else if (i >= 2 && myMap.gateTwo.activeInHierarchy)
                            {

                                return false;
                            }
                        }
                        else if (myMap.isLevelFour())
                        {
                            if (i == 0 && myMap.l4GateOne.activeInHierarchy)
                            {
                                return false;
                            }
                            else if ((i == 1 || i == 2) && myMap.l4GateTwo.activeInHierarchy)
                            {

                                return false;
                            }
                            else if ((i == 3 || i == 4) && myMap.l4GateThree.activeInHierarchy)
                            {

                                return false;
                            }
                            else if (i == 5 && myMap.l4GateFour.activeInHierarchy)
                            {

                                return false;
                            }
                        }
                        else if (myMap.isLevelFive())
                        {
                            if (i == 0 && myMap.l5GateOne.activeInHierarchy)
                            {
                                return false;
                            }
                            else if (i == 1 && myMap.l5GateTwo.activeInHierarchy)
                            {

                                return false;
                            }
                        }
                        else if (myMap.isLevelSix())
                        {
                            if ((i == 0 || i == 1) && myMap.l6GateOne.activeInHierarchy)
                            {
                                return false;
                            }
                            else if (i == 2 && myMap.l6GateTwo.activeInHierarchy)
                            {

                                return false;
                            }
                            else if (i >= 3 && i <= 9 && myMap.l6GateThree.activeInHierarchy)
                            {

                                return false;
                            }
                            else if (i >= 10 && i <= 13 && myMap.l6GateFour.activeInHierarchy)
                            {

                                return false;
                            }
                        }
                        else if (myMap.isLevelSeven())
                        {
                            if ((i == 0 || i == 1) && myMap.l7GateOne.activeInHierarchy)
                            {
                                return false;
                            }
                            else if ((i == 2 || i == 3) && myMap.l7GateTwo.activeInHierarchy)
                            {

                                return false;
                            }
                            else if ((i == 4 || i == 5) && myMap.l7GateThree.activeInHierarchy)
                            {

                                return false;
                            }
                            else if ((i == 6 || i == 7) && myMap.l7GateFour.activeInHierarchy)
                            {

                                return false;
                            }
                        }
                    }
                }
            }
        }

        // Determine if car goes out from the correct exit
        // Car can't go out from the wrong exit
        if (isExit((int)rightx, ny))
    {
        if (myMap.multi_exit_pos_set.Count == 1)
        {
            updateTips();
        }
        else if (myMap.multi_exit_pos_set.Count == 2)
        {
            string label = gameObject.name.Substring(0, 1);
            Vector3 exitPos = myMap.getExitPos();
            int exitOneX = (int)exitPos[0];
            int exitOneY = (int)exitPos[1];
            List<KeyValuePair<int, int>> exitPos2 = myMap.multi_exit_pos_set;
            int exitTwoX = (int)exitPos2[0].Key;
            int exitTwoY = (int)exitPos2[0].Value;
            if (string.Equals(label, "A") && (int)rightx == exitTwoX && ny == exitTwoY)
            {
                updateTips();
            }
            else if (string.Equals(label, "B") && (int)rightx == exitOneX && ny == exitOneY)
            {
                updateTips();
            }
                else
                {
                    return false;
                }
            }
    }

        if (isBelt()){
        moveCarOnBelt(nx, ny, (int)rightx, (int)leftx);
      } else {
        moveCar(nx, ny, (int)rightx, (int)leftx);
      }

      if (isExit((int)rightx, ny))
      {
            if(!minusTip){
                myGameManager.showTip10();
            }
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
        if (myMap.isLevelThree() || myMap.isLevelFour() || myMap.isLevelFive() || myMap.isLevelSix() || myMap.isLevelSeven())
        {
            carInLot((int)leftx, (int)rightx, ny, newLeftX, newRightX, (int)oldLocation.y);
        }
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
        // checkOld == 2 the car was in a gate lot, but now the car is not in the gate lot
        string oldPosText = "";
        foreach (var lot in myMap.gate_lot_pos)
        {
            if (lot.Key == oldCarLeftX && lot.Value == oldCartY)
            {
                checkOld++;
                oldPosText = posToTag(oldCarLeftX) + oldPosText;
            }
            else if (lot.Key == oldCarRightX && lot.Value == oldCartY)
            {
                checkOld++;
                oldPosText += posToTag(oldCarRightX) + posToTag(oldCartY);
            }
            if (checkOld == 2)
            {
                closeGate(oldPosText);
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
                if (myMap.isLevelThree())
                {
                    if (i == 0) myMap.gateOne.SetActive(false);
                    if (i == 1) myMap.gateTwo.SetActive(false);
                }
                else if (myMap.isLevelFour())
                {
                    if (i == 0) myMap.l4GateOne.SetActive(false);
                    if (i == 1) myMap.l4GateTwo.SetActive(false);
                    if (i == 2) myMap.l4GateThree.SetActive(false);
                    if (i == 3) myMap.l4GateFour.SetActive(false);
                }
                else if (myMap.isLevelFive())
                {
                    if (i == 0) myMap.l5GateOne.SetActive(false);
                    if (i == 1) myMap.l5GateTwo.SetActive(false);
                }
                else if (myMap.isLevelSix())
                {
                    if (i == 0) myMap.l6GateOne.SetActive(false);
                    if (i == 1) myMap.l6GateTwo.SetActive(false);
                    if (i == 2) myMap.l6GateThree.SetActive(false);
                    if (i == 3) myMap.l6GateFour.SetActive(false);
                }
                else if (myMap.isLevelSeven())
                {
                    if (i == 0) myMap.l7GateOne.SetActive(false);
                    if (i == 1) myMap.l7GateTwo.SetActive(false);
                    if (i == 2) myMap.l7GateThree.SetActive(false);
                    if (i == 3) myMap.l7GateFour.SetActive(false);
                }
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
                if (myMap.isLevelThree())
                {
                    if (i == 0) myMap.gateOne.SetActive(true);
                    if (i == 1) myMap.gateTwo.SetActive(true);
                }
                else if (myMap.isLevelFour())
                {
                    if (i == 0) myMap.l4GateOne.SetActive(true);
                    if (i == 1) myMap.l4GateTwo.SetActive(true);
                    if (i == 2) myMap.l4GateThree.SetActive(true);
                    if (i == 3) myMap.l4GateFour.SetActive(true);
                }
                else if (myMap.isLevelFive())
                {
                    if (i == 0) myMap.l5GateOne.SetActive(true);
                    if (i == 1) myMap.l5GateTwo.SetActive(true);
                }
                else if (myMap.isLevelSix())
                {
                    if (i == 0) myMap.l6GateOne.SetActive(true);
                    if (i == 1) myMap.l6GateTwo.SetActive(true);
                    if (i == 2) myMap.l6GateThree.SetActive(true);
                    if (i == 3) myMap.l6GateFour.SetActive(true);
                }
                else if (myMap.isLevelSeven())
                {
                    if (i == 0) myMap.l7GateOne.SetActive(true);
                    if (i == 1) myMap.l7GateTwo.SetActive(true);
                    if (i == 2) myMap.l7GateThree.SetActive(true);
                    if (i == 3) myMap.l7GateFour.SetActive(true);
                }
            }
        }
    }

    private void Awake()
    {
        myMap = FindObjectOfType<MapCreater>();
        myGameManager = FindObjectOfType<GameManager>();
        myTipPosition = FindObjectOfType<tipPosition>();

    }

    protected bool isCar(int x, int y){
        return myMap.getCarsPosSet().Contains(myMap.TwoDToOneD(x,y));
    }


    protected bool isWall(int x, int y)
    {
        return myMap.getWallPosSet().Contains(myMap.TwoDToOneD(x, y));
    }


    protected bool isGate(int x, int y)
    {
        foreach (var pos in myMap.gate_pos)
        {
            if (pos.Key == x && pos.Value == y)
            {
                return true;
            }
        }
        return false;
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
        // First Exit
        Vector3 exitPos = myMap.getExitPos();
        if (exitPos[0] == x && exitPos[1] == y ){

           return true;
        }
        // Second Exit
        List<KeyValuePair<int, int>> exitPos2 = myMap.multi_exit_pos_set;
        if (exitPos2[0].Key == x && exitPos2[0].Value == y)
        {
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
