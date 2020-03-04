using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class displayTips : MonoBehaviour
{
    public Text totalTipstext;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      float curTips = PlayerPrefs.GetFloat("totalTips", 0);
      totalTipstext.text = "Tips: " + curTips;
    }
}
