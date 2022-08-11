using UnityEngine;
using UnityEngine.Profiling;
public class calibrationWithKabsch : MonoBehaviour
{
    public Transform[] inPoints;
    public Transform[] referencePoints;
    Vector3[] points; Vector4[] refPoints;
    KabschSolver solver = new KabschSolver();
    public GameObject Calibration;
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
            Matrix4x4 kabschTransform = solver.SolveKabsch(points, refPoints);
            Calibration.transform.position = kabschTransform.MultiplyPoint(Vector3.zero);
            Calibration.transform.rotation = kabschTransform.rotation;
            for (int i = 0; i < inPoints.Length; i++)
            {
                inPoints[i].position = kabschTransform.MultiplyPoint(points[i]);
            }
        }
    }
}