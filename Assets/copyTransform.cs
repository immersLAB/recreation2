using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class copyTransform : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameObject.Find("(Measurement Probe  Marker: " + this.gameObject.name + ")") != null)
        {
            this.transform.position = GameObject.Find("(Measurement Probe  Marker: " + this.gameObject.name + ")").transform.position;
        }
        Debug.Log("(Measurement Probe  Marker:" + this.gameObject.name + ")");
       
       
    }
}
