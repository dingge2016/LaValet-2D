using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trainController : CarControl {
    //private GameObject gameObject = GameObject.Find("Train1");
    private bool horizonalDirection;
    private float width;
    private float height;
    private float gridLength = 1;
    private int TopLeft_x;
    private int TopLeft_y;
    private List<int> carPositions;

    void Start()
    {
        horizonalDirection = gameObject.transform.rotation.z == 0;
        if (horizonalDirection)
        {
            width = Mathf.Ceil(gameObject.transform.localScale.x);
            height = Mathf.Ceil(gameObject.transform.localScale.y);
        }
        else
        {
            width = Mathf.Ceil(gameObject.transform.localScale.y);
            height = Mathf.Ceil(gameObject.transform.localScale.x);
        }

        TopLeft_x = (int)(gameObject.transform.position.x - (float)width / 2 + gridLength / 2);
        TopLeft_y = (int)(gameObject.transform.position.y - (float)height / 2 + gridLength / 2);

        for (int i = 0; i < (int)width; i++)
        {
            for (int j = 0; j < (int)height; j++)
            {
                myMap.addTrains(TopLeft_x + i, TopLeft_y + j);
                myMap.addCars(TopLeft_x + i, TopLeft_y + j);
            }
        }

    }

    //used update and fixedupdate from carcontrol, used for joystick
    void Update()
    {
        //raycast used to select car
        if (Input.GetMouseButtonDown(0)){
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10;

            Vector3 screenPos = Camera.main.ScreenToWorldPoint(mousePos);

            RaycastHit2D hit = Physics2D.Raycast(screenPos,Vector2.zero);

            if(hit)
            {
                //save name of the object
                name = hit.collider.name;
            }
        }

            if(name==this.gameObject.name){
                //returns true during the frame the mouse is pressed down
                if(Input.GetMouseButtonDown(0)){
                    pointA = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
                    //Debug.Log("pointA" + pointA);
                    control.circle.transform.position = originalPos;
                }
                //mouse is held down
                if(Input.GetMouseButton(0)){
                    touchStart = true;
                    pointB = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
                    //Debug.Log("pointB" + pointB);
                }
                else{
                    touchStart = false;
                }
            }
    }

    private void FixedUpdate(){
        if(touchStart){
            Vector3 offset = pointB-pointA;
            movetheSelectedCar(offset);
            //limit movement to 1.0f so that button doesn't go out of joystick
            Vector3 direction = Vector3.ClampMagnitude(offset,1.0f);
            control.circle.transform.position = new Vector3(originalPos.x + direction.x, originalPos.y + direction.y, 0);
        }
        else{
            control.circle.transform.position = originalPos;
        }
    }

    public override void movetheSelectedCar(Vector3 curPosition)
    {


      //  horizonalDirection = gameObject.transform.rotation.x == 0;
        float diff_x = 0, diff_y = 0;
        if (horizonalDirection) // the train can be moved horizonally
        {
            //changed diff_x and diff_y to work with joystick control
            diff_x = curPosition.x;
           //diff_x = curPosition.x - gameObject.transform.position.x;
        }
        else  // the train can be moved vertically
        {
            diff_y = curPosition.y;
           //diff_y = curPosition.y - gameObject.transform.position.y;
        }

        var dxAndDy = flattenDiff(diff_x, diff_y);

        int dx = dxAndDy.Key;
        int dy = dxAndDy.Value;
        if (dx == 0 && dy == 0)
            return;


        int next_x, next_y;

        TopLeft_x = (int)(gameObject.transform.position.x - (float)width / 2 + gridLength / 2);
        TopLeft_y = (int)(gameObject.transform.position.y - (float)height / 2 + gridLength / 2);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                next_x = TopLeft_x + i + dx;
                next_y = TopLeft_y + j + dy;


                if ((dx == 1 && i == width -1) || (dx == -1 && i == 0) || (dy == 1 && j == height-1) || (dy == -1 && j == 0))
                {
                    if (isWall(next_x, next_y) || isCar(next_x, next_y) || isExit(next_x, next_y))
                    {
                        return;
                    }

                }
            }
        }

        float nx = gameObject.transform.position.x + dx + centerOffset;
        float ny = gameObject.transform.position.y + dy;
        transform.position = new Vector3(nx, ny);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                myMap.removeTrains(TopLeft_x + i, TopLeft_y + j);
                myMap.removeCars(TopLeft_x + i, TopLeft_y + j);
            }
        }
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                myMap.addTrains(TopLeft_x + i + dx, TopLeft_y + j + dy);
                myMap.addCars(TopLeft_x + i + dx, TopLeft_y + j + dy);
            }
        }




    }
}
