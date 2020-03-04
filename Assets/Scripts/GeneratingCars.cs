using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneratingCars: MonoBehaviour
{
    public float spawnTime = 1f;
    public GameObject car;
    public GameObject start;
    public GameObject initial;
    //use for naming the car
    private int nextNameNumber=0;
    private int objNameNumber=0;
    private string name;
    public List<GameObject> theCars;
    public bool breakFlag;
    public HashSet<int> set = new HashSet<int>();
    private float leftOffSet = -0.5f;
    private float rightOffSet = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        //add initial location of car to the set
        initial = GameObject.Find("ACarObject");
        set.Add(TwoDToOneD((int)(initial.transform.position.x+leftOffSet),(int)initial.transform.position.y));
        set.Add(TwoDToOneD((int)(initial.transform.position.x+rightOffSet),(int)initial.transform.position.y));
  
        //start is an empty game object that indicates where to place the newly generated car
    	start = GameObject.Find("Start");

    }

    //create new car objects
    private void createCar(){
    	GameObject newCar = Instantiate(car) as GameObject;
        newCar.SetActive(true);
        theCars.Add(newCar);
        newCar.AddComponent<CarControl>();
        newCar.name = "longCar" + nextNameNumber;
    	newCar.transform.position = new Vector3(start.transform.position.x, start.transform.position.y);
        
        //store initial left side and right side locatioins of cars into hash set 
        float startLeft = newCar.transform.position.x + leftOffSet;
        float startRight = newCar.transform.position.x + rightOffSet;
        float startY = newCar.transform.position.y;
        set.Add(TwoDToOneD((int)startLeft,(int)startY));
        set.Add(TwoDToOneD((int)startRight,(int)startY));
    
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

    void Update(){
            //create new car if car at initial position has moved 

            //check if initial car has moved
            if(breakFlag==false){
                if(initial.transform.position!=start.transform.position){
                    StartCoroutine(WaitForASecond()); 
                    Debug.Log(nextNameNumber.ToString());
                    nextNameNumber++;
                    breakFlag = true;
                }
            }
            //avoid index out of range 
            if(theCars.Count==0 || objNameNumber+1>theCars.Count){
                return;
            }

            //check if later created cars have moved 
            if(theCars[objNameNumber].transform.position!=start.transform.position){
                StartCoroutine(WaitForAnotherSecond());
                nextNameNumber++;
                objNameNumber++;
                Debug.Log(objNameNumber.ToString());
            } 
    }
}
