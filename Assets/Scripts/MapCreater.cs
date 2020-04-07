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
    public GameObject Belt;
    public GameObject Button;
    public GameObject VerticalTrack;
    public GameObject IntersectionTrack;
    public GameObject HorizontalTrack;
    public GameObject gateOne;
    public GameObject gateTwo;

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
    private Vector3 exitPos;
    private Vector3 entranceBarrierPos;
    private Vector3 btnPos;

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
        gateOne = GameObject.Find("GateOne");
        gateTwo = GameObject.Find("GateTwo");
        gateInLevel();

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
                else if (row[i] == 'B')
                {
                   belts.Add(Instantiate(Belt, cell_pos, Quaternion.identity));
                   belt_pos_set.Add(TwoDToOneD(row_pos, col_pos));
                }
                else if (row[i] == 'T')
                {
                    Instantiate(Button, cell_pos, Quaternion.identity);
                    btnPos = new Vector3(row_pos, col_pos);
                }
                col_pos++;
            }
            row_pos++;
        }


        // Add gate positions to gate_pos list
        gate_pos.Add(new KeyValuePair<int, int>(2, 1));   // gate one left
        gate_pos.Add(new KeyValuePair<int, int>(3, 1));   // gate one right
        gate_pos.Add(new KeyValuePair<int, int>(2, -1));   // gate two left
        gate_pos.Add(new KeyValuePair<int, int>(3, -1));   // gate two right


        myDia = FindObjectOfType<DialogueManager>();
        createCar();

        // if more than one entrance, generate car in second entrance using createCar2()
        if (multi_ent_pos_set.Count == 2)
        {
            createCar2();
        }

        //foreach (var item in multi_exit_pos_set)
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

    // Check if there is any gate in the level
    public bool gateInLevel()
    {
        if (gateOne == null && gateTwo == null)
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
      return car_pos_set.Contains(TwoDToOneD((int)btnPos[0], (int)btnPos[1]));
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
