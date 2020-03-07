using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
  public static float totalTips = 0;
  public int totalTime;
  public GameObject winUI;
  public GameObject loseUI;

  private int requireTip = 5;
  private float currentTime = 0f;

  void Start(){
    GameObject.Find("Canvas/tipsGoal").GetComponent<Text>().text = "Tips Goal: " + requireTip.ToString();
    currentTime = totalTime;
  }

  void Update(){
      //Time Display
      currentTime -= 1 * Time.deltaTime;     // Time.deltaTime to make time be updated by second not by frame
      int seconds = (int)(currentTime % 60);
      int minutes = (int)(currentTime / 60);
      GameObject.Find("Canvas/timerText").GetComponent<Text>().text = string.Format("{0:0}:{1:00}", minutes, seconds);
      if (currentTime <= 0)
      {
          currentTime = 0;
      }
      if (currentTime == 0)
      {
          EndGame();
          enabled = false;
      }
      //Tips Display
      GameObject.Find("Canvas/tipsText").GetComponent<Text>().text = "Tips:" + totalTips.ToString();
  }

  public void EndGame(){
    if (totalTips >= requireTip){
      winUI.SetActive(true);
      GameObject.Find("Canvas/winUI/tipsAmount").GetComponent<Text>().text = totalTips.ToString();
      // Coins + 1
      int curCoin = PlayerPrefs.GetInt("coins", 0);
      PlayerPrefs.SetInt("coins", curCoin+1);
      GameObject.Find("Canvas/winUI/coinAmount").GetComponent<Text>().text = (curCoin+1).ToString();
      // Reach new level
      int reachLevel = PlayerPrefs.GetInt("curLevel", 1);
      int curLevel = SceneManager.GetActiveScene().buildIndex-2;;
      PlayerPrefs.SetInt("curLevel", Mathf.Min(curLevel,reachLevel)+1);
    } else {
      loseUI.SetActive(true);
      GameObject.Find("Canvas/loseUI/tipsAmount").GetComponent<Text>().text = (requireTip - totalTips).ToString();
    }
  }

  public void Restart(){
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  }

  public void NextLevel(){
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
  }

  public void Menu(){
    SceneManager.LoadScene(0);
  }

  public void Reset(){
    // Reset the Amount of Coins;
    PlayerPrefs.SetInt("coins", 0);
  }
}
