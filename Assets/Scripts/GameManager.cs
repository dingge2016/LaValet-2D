using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{
    protected DialogueManager myDia;
    public static float totalTips = 0;
    public int totalTime;
    public GameObject winUI;
    public GameObject loseUI;
    public GameObject storeUI;
    public GameObject dialogueUI;
    public int requireTip = 5;
    private float currentTime = 0f;
    // props related variable
    private int[] propsPrice;
    private string[] propsName;
    private bool[] propsStatus;
    // judge whether the scence is initialized to avoid unnecessary computation.
    private bool finishGame;
    private bool finishDialogues;
    private bool destroyStore;



    // for prop2 : double Tips for 5 second
    public GameObject doubleTipsPropButtion;
    private float doubleTipsEndTime;
    private bool duringDoubleTipsTime;

    // for selected car;
    private GameObject selectedCar;
    public GameObject driver;


    void Start()
    {
        selectedCar = null;
        duringDoubleTipsTime = false;
        PlayerPrefs.SetInt("coins", 5);
        totalTips = 0;
        destroyStore = false;
        finishDialogues = false;
        finishGame = false;
        propsPrice = new int[] { 1, 1, 1 };
        propsName = new string[] { "Increasing game time", "Car Bomb", "Double Tips for 5 second" };
        propsStatus = new bool[] { false, false, false };
        GameObject.Find("Canvas/tipsGoal").GetComponent<Text>().text = "Tips Goal: " + requireTip.ToString();
        currentTime = totalTime;
        myDia = FindObjectOfType<DialogueManager>();

        storeUI.SetActive(true);
        int curCoin = PlayerPrefs.GetInt("coins", 0);
        GameObject.Find("Canvas/storeUI/coinAmount").GetComponent<Text>().text = (curCoin).ToString();

        //Disable Car & Dialogue
        (GameObject.Find("DialogueManager").GetComponent("DialogueManager") as MonoBehaviour).enabled = false;
        setCarObjectStatus(false);
    }

    public void ClickEnterGameButton()
    {
        //Enter Game From Store
        dialogueUI.SetActive(true);
        Destroy(storeUI);
        destroyStore = true;
    }

    void setCarObjectStatus(bool status)
    {
        if (GameObject.Find("ACarObject0")){
          (GameObject.Find("ACarObject0").GetComponent("CarTimer") as MonoBehaviour).enabled = status;
          (GameObject.Find("ACarObject0").GetComponent("CarControl") as MonoBehaviour).enabled = status;
        }
    }

    void Update()
    {

         
        setdriverPosition(); 

        // Waiting for Game Finish
        if (finishGame){
          setCarObjectStatus(false);
          return;
        }

        // Waiting for Store Finish
        if (!destroyStore)//Inside Store
        {
            return;
        } else {
          (GameObject.Find("DialogueManager").GetComponent("DialogueManager") as MonoBehaviour).enabled = true;
          setCarObjectStatus(true);
        }

        // Waiting for Instruction Dialogue Finish
        if (!myDia.getFinishFlag()){
          return;
        }

        //Time Display
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

        if (currentTime < doubleTipsEndTime)
        {
            GameObject.Find("Main Camera").GetComponent<Camera>().backgroundColor = new Color(70f / 255f, 70f / 255f, 70f / 255f);
            duringDoubleTipsTime = false;
            doubleTipsPropButtion.SetActive(false);
        }
         
       
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
            int curLevel = SceneManager.GetActiveScene().buildIndex - 1;
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
        if (propId == 2)
        {
            doubleTipsPropButtion.SetActive(true);
        }


    }

    // onclick function for the prop2 : double tips for 5s
    public void doubleTipsfor5s()
    {
        if (propsStatus[2])
        {
            duringDoubleTipsTime = true;
            doubleTipsEndTime = currentTime - 5;
            GameObject.Find("Main Camera").GetComponent<Camera>().backgroundColor = new Color(100f / 255f, 0f / 255f, 0f / 255f);
            propsStatus[2] = false;
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
        totalTips = 0;
        SceneManager.LoadScene(0);
    }

    public void Reset()
    {
        // Reset the Amount of Coins;
        PlayerPrefs.SetInt("coins", 0);
    }


    // update selected and the border of selectedCar
    public void setSelectedCar(GameObject selected_car)
    {
        if (selectedCar == selected_car)
            return;


        // change the border of the unselected car into transparent
        if (selectedCar != null)
        {
            selectedCar.GetComponent<Renderer>().enabled = false;
        }

        
        selectedCar = selected_car;
        if (selectedCar == null)
        { 
            driver.GetComponent<Renderer>().enabled = false;
        }

        else {
             
            driver.GetComponent<Renderer>().enabled = true;

            // change the border of the selected car into white;
            selectedCar.GetComponent<Renderer>().enabled = true;
            Debug.Log(selectedCar.GetComponent<Renderer>().material.color);
            selectedCar.GetComponent<Renderer>().material.color = Color.black;

        } 
    }

    void setdriverPosition()
    { 
        if (selectedCar != null)
        {
            driver.transform.position = new Vector3(selectedCar.transform.position.x, selectedCar.transform.position.y, -7); 
        }
        
    }
    public GameObject getSelectedCar()
    {
        return selectedCar;
    }

    public bool isDuringDoubleTipsTime()
    {
        return duringDoubleTipsTime;
    }

}
