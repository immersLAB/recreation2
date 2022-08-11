using UnityEngine;
using UnityEngine.Profiling;
public class Calibration : MonoBehaviour
{
    public Transform[] inPoints;
    public Transform[] referencePoints;
    public bool tracking = false;
    Vector3[] points; Vector4[] refPoints;
    KabschSolver solver = new KabschSolver();
    public GameObject probe;
    public Matrix4x4 calibrationTransform = Matrix4x4.identity;
    //Set up the Input Points
    void Start()
    {
        points = new Vector3[inPoints.Length];
        refPoints = new Vector4[inPoints.Length];
        for (int i = 0; i < inPoints.Length; i++)
        {
            points[i] = inPoints[i].position;
        }
    }

    //Calculate the Kabsch Transform and Apply it to the input points
    void Update()
    {
        for (int i = 0; i < inPoints.Length; i++)
        {
            refPoints[i] = new Vector4(referencePoints[i].position.x, referencePoints[i].position.y, referencePoints[i].position.z, referencePoints[i].localScale.x);
        }
        if (Input.GetKeyDown("space"))
        {
            calibrationTransform =  solver.SolveKabsch(points, refPoints);
            //this.transform.rotation = kabschTransform.rotation;

            for (int i = 0; i < inPoints.Length; i++)
            {
                inPoints[i].position = calibrationTransform.MultiplyPoint3x4(points[i]);

            }

        }

        
    }
}
