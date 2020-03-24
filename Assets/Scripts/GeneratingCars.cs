using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneratingCars: MonoBehaviour
{
    private MapCreater myMap;
    public GameObject car;
    private GameObject initial;
    //use for naming the car
    private GameObject myCar;
    private int nextNameNumber=0;
    private int objNameNumber=0;
    public List<GameObject> theCars;
    public bool breakFlag = true;
    public HashSet<int> set = new HashSet<int>();
    private float leftOffSet = -0.5f;
    private float rightOffSet = 0.5f;
    private Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        myMap = FindObjectOfType<MapCreater>();
        startPos = myMap.getEntranceBarrierPos();
        Debug.Log(startPos);
        initial = createCar();
    }

    //create new car objects
    private GameObject createCar(){


        GameObject newCar = Instantiate(car) as GameObject;

        //store initial left side and right side locatioins of cars into hash set
        newCar.transform.position = startPos;
        Debug.Log(newCar.transform.position);
        float startLeft = newCar.transform.position.x + leftOffSet;
        float startRight = newCar.transform.position.x + rightOffSet;
        float startY = newCar.transform.position.y;
        set.Add(TwoDToOneD((int)startLeft, (int)startY));
        set.Add(TwoDToOneD((int)startRight, (int)startY));

        newCar.SetActive(true);
        theCars.Add(newCar);
        newCar.name = "ACarObject"+nextNameNumber;
        nextNameNumber++;

        return newCar;

    }

    //wait one second to generated new car
    private IEnumerator WaitForASecond(){
            yield return new WaitForSeconds(1f);
            createCar();
    }

    private IEnumerator WaitForAnotherSecond(){
            yield return new WaitForSeconds(1f);
            createCar();
    }

    //convert 2d position to 1d value
    public int TwoDToOneD(int x, int y) {
        return 1000 * x + y;
    }


    //create new car if car at initial position has moved
    void Update(){

            if(theCars.Count==0 || objNameNumber+1>theCars.Count){
                return;
            }

            //check if later created cars have moved
            if(theCars[objNameNumber].transform.position != startPos &&
            theCars[objNameNumber].transform.position.x != startPos[0]+1)
        {
                StartCoroutine(WaitForASecond());
                Debug.Log(nextNameNumber.ToString());
                objNameNumber++;
            }
    }
}
