using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsControl : MonoBehaviour
{
    // Start is called before the first frame update

    List<bool> propsState;
    private int numOfProps = 1;
    void Start()
    {
        propsState = new List<bool>(1);
     
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void AddProRequest(int propId)
    {
        propsState[propId] = true;
        GameObject propButton = GameObject.Find("prop"+propId.ToString());
        propButton.GetComponent<Image>().color = Color.green;
        
    }
}
