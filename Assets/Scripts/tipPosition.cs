using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tipPosition : MonoBehaviour
{
    // Start is called before the first frame update
    
    public Vector3 tipPos;
    // Update is called once per frame
    void Update()
    {
        tipPos = Camera.main.WorldToScreenPoint(this.transform.position);
    }
}
