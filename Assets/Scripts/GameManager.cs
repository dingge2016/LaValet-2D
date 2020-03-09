using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{
    public static float totalTips = 0;
    public int totalTime;
    public GameObject winUI;
    public GameObject loseUI;
    public GameObject storeUI;
    public int requireTip = 5;
    private float currentTime = 0f;
    // props related variable
    private int[] propsPrice;
    private string[] propsName;
    private bool[] propsStatus;
    // judge whether the scence is initialized to avoid unnecessary computation.
    private bool enterStore;
    private bool finishGame;
    void Start()
    {
        finishGame = false;
        enterStore = false;
        propsPrice = new int[] {1};
        propsName = new string[] { "Increasing game time" };
        propsStatus = new bool[] { false };
        GameObject.Find("Canvas/tipsGoal").GetComponent<Text>().text = "Tips Goal: " + requireTip.ToString();
        currentTime = totalTime;

    }

    public void ClickEnterGameButton()
    {
        Debug.Log("here");
        setCarObjectStatus(true);
        Destroy(storeUI);

    }

    // disable store before entering game
    // !!!error: cannot enabled carcontrol
    void setCarObjectStatus(bool status)
    {
        enabled = status;
        (GameObject.Find("ACarObject0").GetComponent("CarTimer") as MonoBehaviour).enabled = status;
        (GameObject.Find("ACarObject0").GetComponent("CarControl") as MonoBehaviour).enabled = status;
    }
    void Update()
    {

        if (finishGame)
            return;


        //Time Display
        if (storeUI != null && !enterStore)
        {
            enterStore = true;
            int curCoin = PlayerPrefs.GetInt("coins", 0);
            Debug.Log("curCoin:"+curCoin.ToString());
            storeUI.SetActive(true);
            GameObject.Find("Canvas/storeUI/coinAmount").GetComponent<Text>().text = (curCoin).ToString();
            setCarObjectStatus(false);

        }
        currentTime -= 1 * Time.deltaTime;     // Time.deltaTime to make time be updated by second not by frame
        int seconds = (int)(currentTime % 60);
        int minutes = (int)(currentTime / 60);
        GameObject.Find("Canvas/timerText").GetComponent<Text>().text = string.Format("{0:0}:{1:00}", minutes, seconds);
        if (currentTime <= 0)
        {
            currentTime = 0;
        }
        if (currentTime == 0 && !finishGame)
        {
            finishGame = true;
            EndGame();
            enabled = false;
        }
        //Tips Display
        GameObject.Find("Canvas/tipsText").GetComponent<Text>().text = "Tips:" + totalTips.ToString();
    }

    public void EndGame()
    {
        if (totalTips >= requireTip)
        {
            winUI.SetActive(true);
            GameObject.Find("Canvas/winUI/tipsAmount").GetComponent<Text>().text = totalTips.ToString();
            // Coins + 1
            int curCoin = PlayerPrefs.GetInt("coins", 0);
            PlayerPrefs.SetInt("coins", curCoin + 1);
            GameObject.Find("Canvas/winUI/coinAmount").GetComponent<Text>().text = (curCoin + 1).ToString();
            // Reach new level
            int reachLevel = PlayerPrefs.GetInt("curLevel", 1);
            int curLevel = SceneManager.GetActiveScene().buildIndex - 2; ;
            PlayerPrefs.SetInt("curLevel", Mathf.Min(curLevel, reachLevel) + 1);
        }
        else
        {
            loseUI.SetActive(true);
            GameObject.Find("Canvas/loseUI/tipsAmount").GetComponent<Text>().text = (requireTip  - totalTips).ToString();
        }
    }



    //  player requests to buy a prop with propId,
    public void PropRequest(int propId)
    {
        int curCoin = PlayerPrefs.GetInt("coins", 0);

        // check whether already buy the prop
        if (propsStatus[propId])
        {
            GameObject.Find("Canvas/storeUI/message").GetComponent<Text>().text = "Already have the prop <" + propsName[propId] + ">";
            return;
        }

        // check whether the player has enough coins to buy the product.
        if (propsPrice[propId] > curCoin)
        {
            GameObject.Find("Canvas/storeUI/message").GetComponent<Text>().text = "Insufficient coins";
            return;
        }

        // update coins and status of props
        PlayerPrefs.SetInt("coins", curCoin  - propsPrice[propId]);
        GameObject.Find("Canvas/storeUI/message").GetComponent<Text>().text = "buy the prop <" + propsName[propId] + "> Successfully.";
        GameObject.Find("Canvas/storeUI/coinAmount").GetComponent<Text>().text = (curCoin - propsPrice[propId]).ToString();
        propsStatus[propId] = true;

        // the props: increasing 15s game time
        if (propId == 0)
        {
            currentTime += 15;
        }


    }
    public void Restart()
    {
        totalTips = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        totalTips = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Menu()
    {
        SceneManager.LoadScene(0);
    }

    public void Reset()
    {
        // Reset the Amount of Coins;
        PlayerPrefs.SetInt("coins", 0);
    }
}
