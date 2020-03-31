using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreater : MonoBehaviour
{
    public string[] map;
    protected DialogueManager myDia;
    public GameObject Upwall;
    public GameObject Downwall;
    public GameObject Leftwall;
    public GameObject Rightwall;
    public GameObject car;

    public GameObject Entry;
    public GameObject Exit;
    public GameObject Ground;
    public GameObject Grass;
    public GameObject Parking;
    public GameObject VerticalTrack;
    public GameObject IntersectionTrack;
    public GameObject HorizontalTrack;

    private HashSet<int> wall_pos_set;
    private HashSet<int> car_pos_set = new HashSet<int>();
    private HashSet<int> train_pos_set = new HashSet<int>();
    public List<KeyValuePair<int, int>> gate_lot_pos = new List<KeyValuePair<int, int>>();
    public List<GameObject> cars;
    private Vector3 exitPos;
    private Vector3 entranceBarrierPos;

    // use to convert 2D position to 1D position.
    public const int SIZE = 1000;

    // Left top position
    public int left_top_x = -5;
    public int left_top_y = 4;

    // Var for generating car
    private int nextNameNumber=0;
    private int objNameNumber=0;
    private float leftOffSet = -0.5f;
    private float rightOffSet = 0.5f;
    private Vector3 startPos;

    private void Awake()
    {
        wall_pos_set = new HashSet<int>();
    }

    // Start is called before the first frame update
    void Start()
    {
        int row_pos = left_top_x;
        foreach (var row in map)
        {
            int col_pos = left_top_y;
            for (int i = 0; i < row.Length; ++i)
            {
                Vector3 cell_pos = new Vector3(row_pos, col_pos);
                if (row[i] == 'U') //Upper Wall
                {
                    Instantiate(Upwall, cell_pos, Quaternion.identity);
                    wall_pos_set.Add(TwoDToOneD(row_pos, col_pos));
                }
                else if (row[i] == 'D') //Down Wall
                {
                    Instantiate(Downwall, cell_pos, Quaternion.identity);
                    wall_pos_set.Add(TwoDToOneD(row_pos, col_pos));
                }
                else if (row[i] == 'L') //Left Wall
                {
                    Instantiate(Leftwall, cell_pos, Quaternion.identity);
                    wall_pos_set.Add(TwoDToOneD(row_pos, col_pos));
                }
                else if (row[i] == 'R') //Right Wall
                {
                    Instantiate(Rightwall, cell_pos, Quaternion.identity);
                    wall_pos_set.Add(TwoDToOneD(row_pos, col_pos));
                }
                else if (row[i] == 'I') //Entry
                {
                    GameObject entry = Instantiate(Entry, cell_pos, Quaternion.identity);
                    entranceBarrierPos = new Vector3(row_pos, col_pos);
                    //          Debug.Log(entranceBarrierPos);
                }
                else if (row[i] == 'O') //Exit
                {
                    GameObject exit = Instantiate(Exit, cell_pos, Quaternion.identity);
                    exitPos = new Vector3(row_pos, col_pos);
                }
                else if (row[i] == '#')
                {
                    Instantiate(Grass, cell_pos, Quaternion.identity);
                }
                else if (row[i] == '.')
                {
                    Instantiate(Ground, cell_pos, Quaternion.identity);
                }

                else if (row[i] == 'P') //Special Lot
                {
                    Instantiate(Parking, cell_pos, Quaternion.identity);
                    gate_lot_pos.Add(new KeyValuePair<int, int>(row_pos, col_pos));
                }

                else if (row[i] == 'V')
                {
                    Instantiate(VerticalTrack, cell_pos, Quaternion.identity);
                }
                else if (row[i] == 'S')
                {
                    Instantiate(IntersectionTrack, cell_pos, Quaternion.identity);
                }
                else if (row[i] == 'H')
                {
                    Instantiate(HorizontalTrack, cell_pos, Quaternion.identity);
                }
                col_pos++;
            }
            row_pos++;
        }
        myDia = FindObjectOfType<DialogueManager>();
        createCar();

        ////check gate lots position
        //foreach (var item in gate_lot_pos)
        //{
        //    Debug.Log(item);
        //    Debug.Log(item.Key);
        //    Debug.Log(item.Value);
        //}
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

    //create new car if car at initial position has moved
    void Update(){
      // Wait for dialogue finish
      if(!myDia.getFinishFlag()){
          return;
      }

      // Wait for Creating Car
      if(cars.Count==0 || objNameNumber + 1 > cars.Count){
          return;
      }

      //check if later created cars have moved
      if(!cars[objNameNumber] || (cars[objNameNumber].transform.position != startPos &&
      cars[objNameNumber].transform.position.x != startPos[0]+1))
      {
          StartCoroutine(WaitForASecond());
          objNameNumber++;
      }
    }

    //create new car objects
    private GameObject createCar(){
        GameObject newCar = Instantiate(car) as GameObject;
        //store initial left side and right side locatioins of cars into hash set
        startPos = new Vector3(entranceBarrierPos[0]+ leftOffSet, entranceBarrierPos[1], -2);
        newCar.transform.position = startPos;
        float startLeft = newCar.transform.position.x + leftOffSet;
        float startRight = newCar.transform.position.x + rightOffSet;
        float startY = newCar.transform.position.y;
        car_pos_set.Add(TwoDToOneD((int)startLeft, (int)startY));
        car_pos_set.Add(TwoDToOneD((int)startRight, (int)startY));
        newCar.SetActive(true);
        cars.Add(newCar);
        newCar.name = "ACarObject"+nextNameNumber;
        if (nextNameNumber == 0){
          FindObjectOfType<CarTimer>().currentTime = 15.0f;
        }
        nextNameNumber++;
        return newCar;
    }

    public HashSet<int> getWallPosSet() {
        return wall_pos_set;
    }

    public HashSet<int> getCarsPosSet() {
        return car_pos_set;
    }

    public HashSet<int> getTrainsPosSet() {
        return train_pos_set;
    }

    public void removeCars(int x, int y){
      car_pos_set.Remove(TwoDToOneD(x,y));
    }

    public void addCars(int x, int y){
      car_pos_set.Add(TwoDToOneD(x,y));
    }

    public void removeTrains(int x, int y){
      train_pos_set.Remove(TwoDToOneD(x,y));
    }

    public void addTrains(int x, int y){
      train_pos_set.Add(TwoDToOneD(x,y));
    }

    public Vector3 getExitPos() {
        return exitPos;
    }

    public Vector3 getEntranceBarrierPos()
    {
        return entranceBarrierPos;
    }

    //Utilitys: ========================
    public void printCarPos(){
      string tmp = "";
      foreach( int pos in getCarsPosSet()){
        tmp += oneDToTwoD(pos);
      }
      Debug.Log(tmp);
    }

    public void printTrainsPos(){
      string tmp = "";
      foreach( int pos in getTrainsPosSet()){
        tmp += oneDToTwoD(pos);
      }
      Debug.Log(tmp);
    }


    public int TwoDToOneD(int x, int y) {
        return SIZE * (x + 100) + (y + 100);
    }

    public string oneDToTwoD(int X) {

        return "(" + ((int)X/SIZE - 100).ToString() + "," + (X%SIZE - 100).ToString()+ ")";
    }
}
