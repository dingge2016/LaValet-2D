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
        horizonalDirection = gameObject.transform.rotation.x == 0;
        width = Mathf.Ceil(gameObject.transform.localScale.x);
        height =Mathf.Ceil(gameObject.transform.localScale.y);


        TopLeft_x = (int)(gameObject.transform.position.x - (float)width / 2 + gridLength/2);
        TopLeft_y = (int)(gameObject.transform.position.y - (float)height / 2 + gridLength/2);
        for (int i = 0; i < (int)width; i++)
        {
            for (int j = 0; j < (int)height; j++)
            {
             //   carPositions.Add(mySet.TwoDToOneD((int)TopLeft_x+i, (int)TopLeft_y+j));
             //myMap(
                myMap.addCars(TopLeft_x + i, TopLeft_y + j);
                Debug.Log(TopLeft_x + i);
                Debug.Log(TopLeft_y + j);
            }
        }

    }


    void Update()
    {

    }
    public override void movetheSelectedCar(Vector3 curPosition)
    {


      //  horizonalDirection = gameObject.transform.rotation.x == 0;
        float diff_x = 0, diff_y = 0;
        if (horizonalDirection) // the train can be moved horizonally
        {
            diff_x = curPosition.x - gameObject.transform.position.x;
        }
        else  // the train can be moved vertically
        {
           diff_y = curPosition.y - gameObject.transform.position.y;
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

                    Debug.Log("dx: " + dx + " dy: " + dy + " next: " + next_x.ToString() + " " + next_y.ToString());
                    Debug.Log(isWall(next_x, next_y));
                    Debug.Log(isCar(next_x, next_y));
                    Debug.Log(isExit(next_x, next_y));


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
                myMap.removeCars(TopLeft_x + i, TopLeft_y + j);
            }
        }
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                myMap.addCars(TopLeft_x + i + dx, TopLeft_y + j + dy);
            }
        }




    }
}
