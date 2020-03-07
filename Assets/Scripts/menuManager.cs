using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class menuManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      GameObject.Find("Canvas/Menu/coinAmount").GetComponent<Text>().text = (PlayerPrefs.GetInt("coins", 0)).ToString();
    }

    public void Reset(){
      // Reset the Amount of Coins;
      PlayerPrefs.SetInt("coins", 0);
      // Reset initial level;
      PlayerPrefs.SetInt("curLevel", 1);
    }
}
