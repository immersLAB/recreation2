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
        string number = this.gameObject.name[this.gameObject.name.Length - 1].ToString();
        if (GameObject.Find("(Measurement Probe  Marker: " + number + ")") != null)
        {
            this.transform.localPosition = GameObject.Find("(Measurement Probe  Marker: " + number + ")").transform.position;
        }
        //Debug.Log(this.transform.position.x);


    }
}
