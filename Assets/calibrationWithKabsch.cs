using UnityEngine;
public class calibrationWithKabsch : MonoBehaviour
{
    public Transform[] inPoints;
    public Transform[] referencePoints;
    Vector3[] points; Vector4[] refPoints;
    KabschSolver solver = new KabschSolver();
    public GameObject Calibration;
    public GameObject CalibrationProbe;
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
        CalibrationProbe.GetComponent<MeshCollider>().enabled = false;
        CalibrationProbe.GetComponent<BoxCollider>().enabled = false;
        for (int i = 0; i < inPoints.Length; i++)
        {
            refPoints[i] = new Vector4(referencePoints[i].position.x, referencePoints[i].position.y, referencePoints[i].position.z, referencePoints[i].localScale.x);
        }
        if (Input.GetKeyDown("space"))
        {
            Vector3 inCentroid = Vector3.zero; Vector3 refCentroid = Vector3.zero;
            float inTotal = 0f, refTotal = 0f;
            for (int i = 0; i < inPoints.Length; i++)
            {
                inCentroid += new Vector3(inPoints[i].position.x, inPoints[i].position.y, inPoints[i].position.z) * refPoints[i].w;
                inTotal += refPoints[i].w;
                refCentroid += new Vector3(refPoints[i].x, refPoints[i].y, refPoints[i].z) * refPoints[i].w;
                refTotal += refPoints[i].w;
            }
            inCentroid /= inTotal;
            refCentroid /= refTotal;
            Vector3 vec = inCentroid - refCentroid;

            Calibration.transform.position = -vec;
            for (int i = 0; i < inPoints.Length; i++)
            {
                points[i] = inPoints[i].position;
            }
            Matrix4x4 kabschTransform = solver.SolveKabsch(points, refPoints);
            Calibration.transform.position = kabschTransform.MultiplyPoint(Vector3.zero);
            Calibration.transform.rotation = kabschTransform.rotation;
            /*for (int i = 0; i < inPoints.Length; i++)
            {
                inPoints[i].position = kabschTransform.MultiplyPoint(points[i]);
            }*/
        }

    }
    public void Calibrate()
    {
        for (int i = 0; i < inPoints.Length; i++)
        {
            refPoints[i] = new Vector4(referencePoints[i].position.x, referencePoints[i].position.y, referencePoints[i].position.z, referencePoints[i].localScale.x);
        }

        Vector3 inCentroid = Vector3.zero; Vector3 refCentroid = Vector3.zero;
        float inTotal = 0f, refTotal = 0f;
        for (int i = 0; i < inPoints.Length; i++)
        {
            inCentroid += new Vector3(inPoints[i].position.x, inPoints[i].position.y, inPoints[i].position.z) * refPoints[i].w;
            inTotal += refPoints[i].w;
            refCentroid += new Vector3(refPoints[i].x, refPoints[i].y, refPoints[i].z) * refPoints[i].w;
            refTotal += refPoints[i].w;
        }
        inCentroid /= inTotal;
        refCentroid /= refTotal;
        Vector3 vec = inCentroid - refCentroid;

        Calibration.transform.position = -vec;
        for (int i = 0; i < inPoints.Length; i++)
        {
            points[i] = inPoints[i].position;
        }
        Matrix4x4 kabschTransform = solver.SolveKabsch(points, refPoints);
        Calibration.transform.position = kabschTransform.MultiplyPoint(Vector3.zero);
        Calibration.transform.rotation = kabschTransform.rotation;
        /*for (int i = 0; i < inPoints.Length; i++)
        {
            inPoints[i].position = kabschTransform.MultiplyPoint(points[i]);
        }*/

    }
}