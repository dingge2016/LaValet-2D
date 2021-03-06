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
    public GameObject Parking1;
    public GameObject Parking2;
    public GameObject Parking3;
    public GameObject Parking4;
    public GameObject Belt;
    public GameObject Button;
    public GameObject VerticalTrack;
    public GameObject IntersectionTrack;
    public GameObject HorizontalTrack;
    public GameObject gateOne;
    public GameObject gateTwo;
    public GameObject l4GateOne;
    public GameObject l4GateTwo;
    public GameObject l4GateThree;
    public GameObject l4GateFour;
    public GameObject l5GateOne;
    public GameObject l5GateTwo;
    public GameObject l6GateOne;
    public GameObject l6GateTwo;
    public GameObject l6GateThree;
    public GameObject l6GateFour;
    public GameObject l7GateOne;
    public GameObject l7GateTwo;
    public GameObject l7GateThree;
    public GameObject l7GateFour;

    private HashSet<int> wall_pos_set;
    private HashSet<int> car_pos_set = new HashSet<int>();
    private HashSet<int> train_pos_set = new HashSet<int>();
    private HashSet<int> belt_pos_set = new HashSet<int>();
    public List<KeyValuePair<int, int>> multi_ent_pos_set = new List<KeyValuePair<int, int>>();
    public List<KeyValuePair<int, int>> multi_exit_pos_set = new List<KeyValuePair<int, int>>();
    public List<KeyValuePair<int, int>> gate_lot_pos = new List<KeyValuePair<int, int>>();
    public List<KeyValuePair<int, int>> gate_pos;
    public List<GameObject> cars;
    public List<GameObject> cars2;
    public List<GameObject> belts;
    private Vector3 btnPos;
    private bool hasBtn = false;
    private Vector3 exitPos;
    private Vector3 entranceBarrierPos;

    // use to convert 2D position to 1D position.
    public const int SIZE = 1000;

    // Left top position
    public int left_top_x = -5;
    public int left_top_y = 4;

    // Var for generating car
    private int nextNameNumber = 0;
    private int nextNameNumber2 = 0;
    private int objNameNumber = 0;
    private int objNameNumber2 = 0;
    private float leftOffSet = -0.5f;
    private float rightOffSet = 0.5f;
    private Vector3 startPos;
    private Vector3 startPos2;


    private void Awake()
    {
        wall_pos_set = new HashSet<int>();
        gate_pos = new List<KeyValuePair<int, int>>();
    }

    // Start is called before the first frame update
    void Start()
    {
        gateOne = GameObject.Find("GateOne");   // Find Gate One in Level Three
        gateTwo = GameObject.Find("GateTwo");   // Find Gate Two in Level Three
        l4GateOne = GameObject.Find("L4GateOne");   // Find Gate One in Level Four
        l4GateTwo = GameObject.Find("L4GateTwo");
        l4GateThree = GameObject.Find("L4GateThree");
        l4GateFour = GameObject.Find("L4GateFour");
        l5GateOne = GameObject.Find("L5GateOne");   // Find Gate One in Level Five
        l5GateTwo = GameObject.Find("L5GateTwo");
        l6GateOne = GameObject.Find("L6GateOne");   // Find Gate One in Level Six
        l6GateTwo = GameObject.Find("L6GateTwo");
        l6GateThree = GameObject.Find("L6GateThree");
        l6GateFour = GameObject.Find("L6GateFour");
        l7GateOne = GameObject.Find("L7GateOne");   // Find Gate One in Level Seven
        l7GateTwo = GameObject.Find("L7GateTwo");
        l7GateThree = GameObject.Find("L7GateThree");
        l7GateFour = GameObject.Find("L7GateFour");

        //Debug.Log("L3: " + isLevelThree());
        //Debug.Log("L4: " + isLevelFour());
        //Debug.Log("L5: " + isLevelFive());
        //Debug.Log("L6: " + isLevelSix());
        //Debug.Log("L7: " + isLevelSeven());

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
                    //Debug.Log(entranceBarrierPos);
                    multi_ent_pos_set.Add(new KeyValuePair<int, int>(row_pos, col_pos));
                }
                else if (row[i] == 'O') //Exit
                {
                    GameObject exit = Instantiate(Exit, cell_pos, Quaternion.identity);
                    exitPos = new Vector3(row_pos, col_pos);
                    multi_exit_pos_set.Add(new KeyValuePair<int, int>(row_pos, col_pos));
                }
                else if (row[i] == '#')
                {
                    Instantiate(Grass, cell_pos, Quaternion.identity);
                }
                else if (row[i] == '.')
                {
                    Instantiate(Ground, cell_pos, Quaternion.identity);
                }

                else if (row[i] == 'P') //Special Lot 1
                {
                    Instantiate(Parking1, cell_pos, Quaternion.identity);
                    gate_lot_pos.Add(new KeyValuePair<int, int>(row_pos, col_pos));
                }

                else if (row[i] == 'E') //Special Lot 2
                {
                    Instantiate(Parking2, cell_pos, Quaternion.identity);
                    gate_lot_pos.Add(new KeyValuePair<int, int>(row_pos, col_pos));
                }

                else if (row[i] == 'F') //Special Lot 3
                {
                    Instantiate(Parking3, cell_pos, Quaternion.identity);
                    gate_lot_pos.Add(new KeyValuePair<int, int>(row_pos, col_pos));
                }

                else if (row[i] == 'G') //Special Lot 4
                {
                    Instantiate(Parking4, cell_pos, Quaternion.identity);
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
                else if (row[i] == 'B')
                {
                   belts.Add(Instantiate(Belt, cell_pos, Quaternion.identity));
                   belt_pos_set.Add(TwoDToOneD(row_pos, col_pos));
                }
                else if (row[i] == 'T')
                {
                    Instantiate(Button, cell_pos, Quaternion.identity);
                    btnPos = new Vector3(row_pos, col_pos);
                    hasBtn = true;
                }
                col_pos++;
            }
            row_pos++;
        }


        // Add gate positions to gate_pos list
        if (isLevelThree())
        {
            gate_pos.Add(new KeyValuePair<int, int>(2, -1));   // level 3 gate one left
            gate_pos.Add(new KeyValuePair<int, int>(3, -1));   // level 3 gate one right
            gate_pos.Add(new KeyValuePair<int, int>(2, 1));    // level 3 gate two left
            gate_pos.Add(new KeyValuePair<int, int>(3, 1));    // level 3 gate two right
        }
        else if (isLevelFour())
        {
            gate_pos.Add(new KeyValuePair<int, int>(2, 2));    // level 4 gate one
            gate_pos.Add(new KeyValuePair<int, int>(-2, 0));   // level 4 gate two left
            gate_pos.Add(new KeyValuePair<int, int>(-1, 0));   // level 4 gate two right
            gate_pos.Add(new KeyValuePair<int, int>(5, 0));    // level 4 gate three left
            gate_pos.Add(new KeyValuePair<int, int>(6, 0));    // level 4 gate three right
            gate_pos.Add(new KeyValuePair<int, int>(2, -2));   // level 4 gate four
        }
        else if (isLevelFive())
        {
            gate_pos.Add(new KeyValuePair<int, int>(1, 0));    // level 5 gate one
            gate_pos.Add(new KeyValuePair<int, int>(7, -3));   // level 5 gate two
        }
        else if (isLevelSix())
        {
            gate_pos.Add(new KeyValuePair<int, int>(1, 2));    // level 6 gate one left
            gate_pos.Add(new KeyValuePair<int, int>(2, 2));    // level 6 gate one right
            gate_pos.Add(new KeyValuePair<int, int>(3, 1));   // level 6 gate two
            gate_pos.Add(new KeyValuePair<int, int>(-2, -2));    // level 6 gate three (1) most left
            gate_pos.Add(new KeyValuePair<int, int>(-1, -2));    // level 6 gate three (2)
            gate_pos.Add(new KeyValuePair<int, int>(0, -2));    // level 6 gate three (3)
            gate_pos.Add(new KeyValuePair<int, int>(1, -2));    // level 6 gate three (4)
            gate_pos.Add(new KeyValuePair<int, int>(2, -2));    // level 6 gate three (5)
            gate_pos.Add(new KeyValuePair<int, int>(3, -2));    // level 6 gate three (6)
            gate_pos.Add(new KeyValuePair<int, int>(4, -2));    // level 6 gate three (7) most right
            gate_pos.Add(new KeyValuePair<int, int>(0, 1));    // level 6 gate four (1) top
            gate_pos.Add(new KeyValuePair<int, int>(0, 0));    // level 6 gate four (2)
            gate_pos.Add(new KeyValuePair<int, int>(0, -1));    // level 6 gate four (3)
            gate_pos.Add(new KeyValuePair<int, int>(0, -2));    // level 6 gate four (4) bottom
        }
        else if (isLevelSeven())
        {
            gate_pos.Add(new KeyValuePair<int, int>(6, 2));   // level 7 gate one left
            gate_pos.Add(new KeyValuePair<int, int>(7, 2));   // level 7 gate one right
            gate_pos.Add(new KeyValuePair<int, int>(0, 2));   // level 7 gate two left
            gate_pos.Add(new KeyValuePair<int, int>(1, 2));   // level 7 gate two right
            gate_pos.Add(new KeyValuePair<int, int>(0, -1));   // level 7 gate three left
            gate_pos.Add(new KeyValuePair<int, int>(1, -1));   // level 7 gate three right
            gate_pos.Add(new KeyValuePair<int, int>(6, -1));   // level 7 gate four left
            gate_pos.Add(new KeyValuePair<int, int>(7, -1));   // level 7 gate four right
        }

        myDia = FindObjectOfType<DialogueManager>();
        createCar();

        // if more than one entrance, generate car in second entrance using createCar2()
        if (multi_ent_pos_set.Count == 2)
        {
            createCar2();
        }

        //foreach (var item in gate_lot_pos)
        //{
        //    Debug.Log(item);
        //    Debug.Log(item.Key);
        //    Debug.Log(item.Value);
        //}
    }

    //wait one second to generated new car for first entrance
    private IEnumerator WaitForASecond(){
            yield return new WaitForSeconds(1f);
        if (!car_pos_set.Contains(TwoDToOneD((int)(startPos[0] + leftOffSet), (int)startPos[1]))
            && !car_pos_set.Contains(TwoDToOneD((int)(startPos[0] + rightOffSet), (int)startPos[1])))
        {
            createCar();
        }
        else objNameNumber--;
    }

    // wait one second to generated new car for second entrance
    private IEnumerator WaitForASecond2()
    {
        yield return new WaitForSeconds(1f);
        if (!car_pos_set.Contains(TwoDToOneD((int)(startPos2[0] + leftOffSet), (int)startPos2[1]))
            && !car_pos_set.Contains(TwoDToOneD((int)(startPos2[0] + rightOffSet), (int)startPos2[1])))
        {
            createCar2();
        }
        else objNameNumber2--;
    }


    void Update(){
        createMoreCar();
        if (multi_ent_pos_set.Count == 2)
        {
            createMoreCar2();
        }
    }


    // create new car at entrance 1 if car at initial position has moved
    void createMoreCar()
    {
        // Wait for Creating Car
        if (cars.Count == 0 || objNameNumber + 1 > cars.Count)
        {
            return;
        }

        //check if later created cars have moved
        if (!car_pos_set.Contains(TwoDToOneD((int)(startPos[0] + leftOffSet), (int)startPos[1]))
          && !car_pos_set.Contains(TwoDToOneD((int)(startPos[0] + rightOffSet), (int)startPos[1])))
        {
            StartCoroutine(WaitForASecond());
            objNameNumber++;
        }
    }

    // create new car at entrance 2 if car at initial position has moved
    void createMoreCar2()
    {
        // Wait for Creating Car
        if (cars2.Count == 0 || objNameNumber2 + 1 > cars2.Count)
        {
            return;
        }

        //check if later created cars have moved
        if (!car_pos_set.Contains(TwoDToOneD((int)(startPos2[0] + leftOffSet), (int)startPos2[1]))
          && !car_pos_set.Contains(TwoDToOneD((int)(startPos2[0] + rightOffSet), (int)startPos2[1])))
        {
            StartCoroutine(WaitForASecond2());
            objNameNumber2++;
        }
    }

    // create new car objects for first entrance
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

    // create new car objects for second entrance
    private GameObject createCar2()
    {
        GameObject newCar = Instantiate(car) as GameObject;
        // store initial left side and right side locatioins of car
        startPos2 = new Vector3(multi_ent_pos_set[0].Key + leftOffSet, multi_ent_pos_set[0].Value, -2);
        newCar.transform.position = startPos2;
        float startLeft = newCar.transform.position.x + leftOffSet;
        float startRight = newCar.transform.position.x + rightOffSet;
        float startY = newCar.transform.position.y;
        car_pos_set.Add(TwoDToOneD((int)startLeft, (int)startY));
        car_pos_set.Add(TwoDToOneD((int)startRight, (int)startY));
        newCar.SetActive(true);
        cars2.Add(newCar);
        newCar.name = "BCarObject" + nextNameNumber2;
        if (nextNameNumber2 == 0)
        {
            FindObjectOfType<CarTimer>().currentTime = 15.0f;
        }
        nextNameNumber2++;
        return newCar;
    }

    // Check if this is level three
    public bool isLevelThree()
    {
        // if we are not able to find GateOne and GateTwo, it means this is not level three
        if (gateOne == null && gateTwo == null)
        {
            return false;
        }
        return true;
    }

    // Check if this is level four
    public bool isLevelFour()
    {
        // if we are not able to find L4GateOne, L4GateTwo, L4GateThree, and L4GateFour, it means this is not level four
        if (l4GateOne == null && l4GateTwo == null && l4GateThree == null && l4GateFour == null)
        {
            return false;
        }
        return true;
    }

    // Check if this is level five
    public bool isLevelFive()
    {
        // if we are not able to find L4GateOne, L4GateTwo, L4GateThree, and L4GateFour, it means this is not level four
        if (l5GateOne == null && l5GateTwo == null)
        {
            return false;
        }
        return true;
    }

    // Check if this is level six
    public bool isLevelSix()
    {
        // if we are not able to find L4GateOne, L4GateTwo, L4GateThree, and L4GateFour, it means this is not level four
        if (l6GateOne == null && l6GateTwo == null && l6GateThree == null && l6GateFour == null)
        {
            return false;
        }
        return true;
    }

    // Check if this is level seven
    public bool isLevelSeven()
    {
        // if we are not able to find L4GateOne, L4GateTwo, L4GateThree, and L4GateFour, it means this is not level four
        if (l7GateOne == null && l7GateTwo == null && l7GateThree == null && l7GateFour == null)
        {
            return false;
        }
        return true;
    }

    public HashSet<int> getWallPosSet() {
        return wall_pos_set;
    }

    public HashSet<int> getBeltPosSet() {
        return belt_pos_set;
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

    public bool isbeltOn(){

      if (hasBtn){
        //Debug.Log("fdshjfdkls" + btnPos);
        return car_pos_set.Contains(TwoDToOneD((int)btnPos[0], (int)btnPos[1]));
      } else {
        return true;
      }

    }

    public List<GameObject> getBelts(){
      return belts;
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
