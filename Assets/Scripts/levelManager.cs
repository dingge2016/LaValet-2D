using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class levelManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      // Display Current Coins
      GameObject.Find("Canvas/levelUI/coinAmount").GetComponent<Text>().text = (PlayerPrefs.GetInt("coins", 0)).ToString();
      // Get Current Level
      int curLevel = PlayerPrefs.GetInt("curLevel", 1); 
        // 1-current level enable button
        for (int i = 1; i <= curLevel; i = i + 1){
        string btnName = "Level " + i.ToString() + " Button";
        GameObject.Find("Canvas/levelUI/"+btnName).GetComponent<Button>().interactable = true;
      }
      // current level-6 disable button
      for (int i = curLevel + 1; i <= 8; i = i + 1){
        string btnName = "Level " + i.ToString() + " Button"; 
        GameObject.Find("Canvas/levelUI/"+btnName).GetComponent<Button>().GetComponent<Image>().color = Color.gray;
        GameObject.Find("Canvas/levelUI/"+btnName).GetComponent<Button>().interactable = false;
      }
    }

    public void Reset(){
      // Reset the Amount of Coins;
      PlayerPrefs.SetInt("coins", 0);
      // Reset initial level;
      PlayerPrefs.SetInt("curLevel", 1);
    }
}
