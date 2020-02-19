using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollorFrontCar : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject FrontCarPart;

    private Vector3 newPosition;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        newPosition.x = FrontCarPart.transform.position.x - 1;
        newPosition.y = FrontCarPart.transform.position.y;

        transform.position = newPosition;

     /*   Debug.Log(newPosition);*/
    }
}
