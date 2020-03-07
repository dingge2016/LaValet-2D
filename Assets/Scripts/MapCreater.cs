using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreater : MonoBehaviour
{
    public string[] map;
  //  {
  //  "..UUUUUU..",
  //  ".L......R.",
  //  ".L......R.",
  //  "UL......RU",
  //  "I........O",
  //  "DL......RD",
  //  ".L......R.",
  //  ".L......R.",
  //  "..DDDDDD.."
  //  };
    public GameObject Upwall;
    public GameObject Downwall;
    public GameObject Leftwall;
    public GameObject Rightwall;

    public GameObject Entry;
    public GameObject Exit;
    public GameObject Ground;
    public GameObject Grass;

    private HashSet<int> wall_pos_set ;
    private Vector3 exitPos;
    private Vector3 entranceBarrierPos;

    // use to convert 2D position to 1D position.
    public const int SIZE = 1000;

    // Left top position
    public int left_top_x = -5;
    public int left_top_y = 4;

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
                col_pos++;
            }
            row_pos++;
        }
    }

    public HashSet<int> getWallPosSet() {
        return wall_pos_set;
    }

    public Vector3 getExitPos() {
        return exitPos;
    }

    public Vector3 getEntranceBarrierPos()
    {
        return entranceBarrierPos;
    }

    public int TwoDToOneD(int x, int y) {
        return SIZE * x + y;
    }
}
