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
    public GameObject initial;
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
    private Vector3 temp2;
    private Vector3 temp3;
    public Text EndGameText;
    public HashSet<int> set = new HashSet<int>();
    private float leftOffSet = -0.5f;
    private float rightOffSet = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        //add initial location of car to the set and dictionary
        initial = GameObject.Find("ACarObject");
        //use to detect overlap 
        set.Add(TwoDToOneD((int)(initial.transform.position.x+leftOffSet),(int)initial.transform.position.y));
        set.Add(TwoDToOneD((int)(initial.transform.position.x+rightOffSet),(int)initial.transform.position.y));

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

        //store initial left side and right side locatioins of cars into hash set 
        float startLeft = newCar.transform.position.x + leftOffSet;
        float startRight = newCar.transform.position.x + rightOffSet;
        float startY = newCar.transform.position.y;
        set.Add(TwoDToOneD((int)startLeft,(int)startY));
        set.Add(TwoDToOneD((int)startRight,(int)startY));

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

    public int TwoDToOneD(int x, int y) {
        return 1000 * x + y;
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

                    //add location of newly generated car to set 
                    float startLeft2 = positions[objNameNumber].x+leftOffSet;
                    float startRight2 = positions[objNameNumber].x+rightOffSet;
                    float startY2 = positions[objNameNumber].y;
                    set.Add(TwoDToOneD((int)startLeft2,(int)startY2));
                    set.Add(TwoDToOneD((int)startRight2,(int)startY2));
                }
                //if there are more than one car, shift current cars up
                else if(objNameNumber+1<currentNumCars){
                    objNameNumber++;
                    count = objNameNumber;
                    temp = positions[count-1];
                    while(count<currentNumCars){
                        //temp3 used to store old location
                        temp3 = theCars[count].transform.position;
                        theCars[count].transform.position = temp;

                        //used to store new location
                        temp2 = temp;
                        temp = positions[count];

                        //new position of each car
                        positions[count] = theCars[count].transform.position;

                        //remove old locations of cars
                        set.Remove(TwoDToOneD((int)(temp3.x+leftOffSet),(int)temp3.y));
                        set.Remove(TwoDToOneD ((int)(temp3.x+rightOffSet),(int)temp3.y));

                        //add new locations after car shift
                        set.Add(TwoDToOneD((int)(temp2.x+leftOffSet),(int)temp2.y));
                        set.Add(TwoDToOneD((int)(temp2.x+rightOffSet),(int)temp2.y));

                        count++;
                    }
                    //make sure the newly generated car shifts as well
                    carLocation = positions[count-1].x-2.0f;
                } 
            }
        }
    }
}

