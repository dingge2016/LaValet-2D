using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;


public class DialogueManager : MonoBehaviour
{
    protected MapCreater myMap;
    private bool finishDialogues = false;
    public GameObject[] dialogues;
    private bool[] dialoguesCompleted = new bool[]{ false, false, false, false };


    void Start()
    {
      //Initialized Dialogues
      myMap = FindObjectOfType<MapCreater>();

    }

    void Update(){
      //Waiting for step1
      if (!dialoguesCompleted[0]) {
        Vector3 entryPos = myMap.getEntranceBarrierPos();
        if (!myMap.getTrainsPosSet().Contains(myMap.TwoDToOneD((int)entryPos[0]+1, (int)entryPos[1]))){ // Step 1 Fullfiled
          dialoguesCompleted[0] = true;
          // Replace
          dialogues[0].SetActive(false);
          dialogues[1].SetActive(true);
        } else{
          return;
        }
      }

      //Waiting for step2
      if (!dialoguesCompleted[1]) {
        Vector3 entryPos = myMap.getEntranceBarrierPos();
        if (!myMap.getCarsPosSet().Contains(myMap.TwoDToOneD((int)entryPos[0], (int)entryPos[1]))){ // Step 2 Fullfiled
          dialoguesCompleted[1] = true;
          // Replace
          dialogues[1].SetActive(false);
          dialogues[2].SetActive(true);
        } else{
          return;
        }
      }

      //Waiting for step3
      if (!dialoguesCompleted[2]) {
        Vector3 exitPos = myMap.getExitPos();
        if (!myMap.getTrainsPosSet().Contains(myMap.TwoDToOneD((int)exitPos[0]-1, (int)exitPos[1]))
              && !myMap.getTrainsPosSet().Contains(myMap.TwoDToOneD((int)exitPos[0]-2, (int)exitPos[1])) ){ // Step 3 Fullfiled
          dialoguesCompleted[2] = true;
          // Replace
          dialogues[2].SetActive(false);
          dialogues[3].SetActive(true);
        } else{
          return;
        }
      }
      //Waiting for step4
      if (!dialoguesCompleted[3]) {
        if (GameObject.Find("ACarObject0") == null){ // Step 4 Fullfiled
          dialoguesCompleted[3] = true;
          // Replace
          dialogues[3].SetActive(false);
          finishDialogues = true;
        } else{
          return;
        }
      }
    }

    public bool getFinishFlag(){
      if (finishDialogues){
        enabled = false;
      }
      return finishDialogues;
    }

    public void Skip()
    {

    }
}
