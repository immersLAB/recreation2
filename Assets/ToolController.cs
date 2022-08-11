using UnityEngine;

public class ToolController : MonoBehaviour
{
    public float speed = 0.01f;

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        float moveInOut = Input.GetAxis("DepthWise");

        transform.Translate(speed * moveInOut, speed * moveHorizontal, speed * moveVertical);

    }
}
