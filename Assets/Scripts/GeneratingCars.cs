using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneratingCars: MonoBehaviour
{
    public float spawnTime = 4f;
    public GameObject car;
    public GameObject start;
    public GameObject lot;
    private float end;
    private int nextNameNumber=0;
    private int objNameNumber;
    private string name;
    private Vector3 lastPos;
    private Vector3 offset;
    private Vector3 entrance;
    private Vector3 boundary;
    private bool carCreated;
    private float carLocation;
    private int count;
    public List<GameObject> theCars;
    public List<Vector3> positions;
    private bool breakFlag;
    private int currentNumCars;
    private Vector3 temp;
    public Text EndGameText;

    // Start is called before the first frame update
    void Start()
    {
        //finds location of the entrance
    	start = GameObject.Find("Start");
    	entrance = new Vector3(start.transform.position.x, start.transform.position.y, start.transform.position.z);
        carLocation = entrance.x;

        //finds location of the boundary of the parking lot 
        lot = GameObject.Find("EndOfEntrance");
        boundary = new Vector3(lot.transform.position.x, lot.transform.position.y, lot.transform.position.z);
        end = boundary.x;

        //initialize parameters for the newly created car object
        name = "longCar";
        objNameNumber=0;
        theCars = new List<GameObject>();
    	StartCoroutine(carWave());

    }

    //create new car objects
    private void createCar(float x){
    	GameObject newCar = Instantiate(car) as GameObject;
        newCar.SetActive(true);
        theCars.Add(newCar);
        newCar.AddComponent<CarControl>();
        newCar.name = "longCar" + nextNameNumber;
    	newCar.transform.position = new Vector3(x, entrance.y, entrance.z);
        //keeps track of car locations
        positions.Add(newCar.transform.position);
    }

    //continuously spawning new car objects
    IEnumerator carWave(){
        breakFlag = false;
    	while(breakFlag==false){
            //wait 10 seconds before generating another car
            yield return new WaitForSeconds(spawnTime);
            createCar(carLocation);
            carCreated = true;
            nextNameNumber++;
            carLocation-=2.0f;
            //if car touches entrance boundary, stop generating cars
            if(carLocation<end){
            	EndGameText.text = "Too many cars at the entrance. Game is over!";
                breakFlag = true;
            }
    	}
    }

    //check on every frame if a car has moved, if so shift cars
    void Update(){
        //after the first car object is created, check for its movement
        if(carCreated==true && objNameNumber<10){
            //original position of car
            lastPos = positions[objNameNumber];
            //new position(if moved)
            offset = theCars[objNameNumber].transform.position - lastPos;
            //used to shift current cars up
            if(offset.x>0.0f){
                currentNumCars = theCars.Count;
                //if there is only one car, no other cars to shift, make sure the next newly generated car shifts up
                if(objNameNumber+1==currentNumCars){
                    carLocation=positions[objNameNumber].x;
                }
                //if there are more than one car, shift current cars up
                else if(objNameNumber+1<currentNumCars){
                    objNameNumber++;
                    count = objNameNumber;
                    temp = positions[count-1];
                    while(count<currentNumCars){
                        theCars[count].transform.position = temp;
                        temp = positions[count];
                        positions[count] = theCars[count].transform.position;
                        count++;
                    }
                    //make sure the newly generated car shifts as well
                    carLocation = positions[count-1].x-2.0f;
                } 
            }
        }
    }
}

