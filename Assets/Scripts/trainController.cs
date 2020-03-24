using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trainController : CarControl {
    //private GameObject gameObject = GameObject.Find("Train1");
    private bool horizonalDirection;
    private float width;
    private float height;
    private float gridLength = 1;
    List<int> carPositions;
    void Start()
    {
        horizonalDirection = gameObject.transform.rotation.x == 0;
        width = Mathf.Ceil(gameObject.transform.localScale.x);
        height =Mathf.Ceil(gameObject.transform.localScale.y);


        float TopLeft_x = gameObject.transform.position.x - (float)width / 2 + gridLength/2;
        float TopLeft_y = gameObject.transform.position.y - (float)height / 2 + gridLength/2;
        for (int i = 0; i < (int)width; i++)
        {
            for (int j = 0; j < (int)height; j++)
            {
             //   carPositions.Add(mySet.TwoDToOneD((int)TopLeft_x+i, (int)TopLeft_y+j));
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
        Debug.Log(gameObject.transform.rotation.x);

        Debug.Log(gameObject.transform.rotation.x == 0);
        Debug.Log(horizonalDirection);
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

        float nx = gameObject.transform.position.x + dx + centerOffset;
        int ny = (int)gameObject.transform.position.y + dy;

        transform.position = new Vector3(nx, ny);


    }
}
