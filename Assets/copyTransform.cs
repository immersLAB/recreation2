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
        string number = this.gameObject.name[this.gameObject.name.Length-1].ToString();
        if (GameObject.Find("(Measurement Probe  Marker: " + number + ")") != null)
        {
            this.transform.position = GameObject.Find("(Measurement Probe  Marker: " + number + ")").transform.position;
        }
        Debug.Log("(Measurement Probe  Marker: " + number + ")");
       
       
    }
}
