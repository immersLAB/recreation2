
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformCalculatons : MonoBehaviour
{
    public GameObject GuideCube;
    public GameObject OptiCube;
    public GameObject HoloLens;
    public GameObject MatchCube;

    public GameObject ToolTip;
    public GameObject OptiTool;
    public GameObject OptiProbe;
    public GameObject USFace;

    private bool averageAquired = false;
    private Matrix4x4 cTh;
    private Matrix4x4 cTo; //This is what we're solving for. We want the opticube as seen by the hololens
    private Vector3 scale = new Vector3(0.09f, 0.09f, 0.09f); //0.095f, 0.095f, 0.095f);
    private Vector3 OneScale = new Vector3(1.0f, 1.0f, 1.0f);
    private Vector3 newPosition;
    private Quaternion newRotation;

    //used for part where NOT USING HEADSET
    private List<Matrix4x4> cTw_List = new List<Matrix4x4>();
    private Matrix4x4 cTw_avg = new Matrix4x4();
    private Matrix4x4 wTo;
    //

    private Vector4 col1 = new Vector4();
    private Vector4 col2 = new Vector4();
    private Vector4 col3 = new Vector4();
    private Vector4 col4 = new Vector4();
    private int counter = 0;
    private List<Vector3> positions = new List<Vector3>() { new Vector3(0.125f, 0, .7f), new Vector3(.3f, 0, .7f), new Vector3(.4f, -.2f, .7f) };
    private List<Quaternion> rotations = new List<Quaternion>() { Quaternion.Euler(30,20,45), Quaternion.Euler(30, 20, 45) };
    private int limit = 10; // can cite another work explaining 10 samples is the optimal amount
    public List<GameObject> targets;
    private List<Transform> transforms;
    public GameObject anchor;
    // This is the code for the transform calculation NOT USING HEADSET
    private void Start()
    {
        transforms = new List<Transform>();
        
    }
    void Update()
    {
        if (Input.GetKeyDown("i"))
        {
            foreach (GameObject target in targets)
            {
                transforms.Add(target.transform);

            }
            anchor.GetComponent<MeshRenderer>().enabled = !anchor.GetComponent<MeshRenderer>().enabled;
            GuideCube.transform.position = transforms[counter].position;
            GuideCube.transform.rotation = transforms[counter].rotation;
            foreach (var item in targets)
            {
                item.SetActive(false);
            }
            GuideCube.SetActive(true);


        }
        if (Input.GetKeyDown("h"))
        {
            MatchCube.GetComponent<MeshRenderer>().enabled = !MatchCube.GetComponent<MeshRenderer>().enabled;
        }
        
        //Calibration Data points by lining up the real world OptiCube to the virtual "Guide Cube" (blue wireframe one)
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("space pressed");
            Matrix4x4 wTo = Matrix4x4.TRS(OptiCube.transform.position, OptiCube.transform.rotation, OptiCube.transform.localScale);
            //Matrix4x4 wTo_LH = convertRHtoLH(wTo); //uncommented for LH -mb

            Matrix4x4 cTg = Matrix4x4.TRS(GuideCube.transform.position, GuideCube.transform.rotation, GuideCube.transform.localScale);

            //HL2GuideCube * OptiCube2Optitrack 
            cTw_List.Add(cTg * wTo.inverse); //commented for LH -mb
           //cTw_List.Add(cTg * wTo_LH.inverse); // added for LH -mb
            Debug.Log("Single Sample Transform: \n" + cTw_List[cTw_List.Count - 1].ToString());

            //just to only have one sample for testing purposes
            //cTw_avg = cTg * wTo.inverse;
            //averageAquired = true;
            counter++;
            if(counter < transforms.Count)
            {
                GuideCube.transform.position = transforms[counter].position;
                GuideCube.transform.rotation = transforms[counter].rotation;
            }
           
        }

        //Once have sufficient sample size,
        if (cTw_List.Count > 9)//&& !averageAquired)
        {
            //Create the average transformation matrix
            for (int row = 0; row < 4; row++)
            {
                Vector4 averageRow = new Vector4();
                for (int transformIndex = 0; transformIndex < cTw_List.Count; transformIndex++)
                {
                    averageRow += cTw_List[transformIndex].GetRow(row);
                }
                averageRow = averageRow / cTw_List.Count;
                cTw_avg.SetRow(row, averageRow);
            }
            Debug.Log("Average Transform: \n" + cTw_avg.ToString());
            averageAquired = true;

        }


        //If you have the average tranformation matrix, use it to position "Match cube" (the black wireframe one)
        if (averageAquired)
        {
            wTo = Matrix4x4.TRS(OptiCube.transform.position, OptiCube.transform.rotation, OptiCube.transform.localScale);
            //Matrix4x4 wTo_LH = convertRHtoLH(wTo); //commented for LH -mb

            cTo = cTw_avg * wTo; //commented for LH -mb
            //cTo = cTw_avg * wTo_LH; //added for LH -mb
            newPosition = new Vector3(cTo.m03, cTo.m13, cTo.m23); 

            newRotation = cTo.rotation;
            MatchCube.transform.localScale = scale;
            MatchCube.transform.SetPositionAndRotation(newPosition, newRotation);

            Matrix4x4 wTt = Matrix4x4.TRS(OptiTool.transform.position, OptiTool.transform.rotation, OptiTool.transform.localScale);
            Matrix4x4 cTt = cTw_avg * wTt;
            newPosition = new Vector3(cTt.m03, cTt.m13, cTt.m23);
            newRotation = cTt.rotation;
            //ToolTip.transform.localScale = scale;
            ToolTip.transform.SetPositionAndRotation(newPosition, newRotation);

            Matrix4x4 wTp = Matrix4x4.TRS(OptiProbe.transform.position, OptiProbe.transform.rotation, OptiProbe.transform.localScale);
            Matrix4x4 cTp = cTw_avg * wTp;
            newPosition = new Vector3(cTp.m03, cTp.m13, cTp.m23);
            newRotation = cTp.rotation;
            //ToolTip.transform.localScale = scale;
            USFace.transform.SetPositionAndRotation(newPosition, newRotation);

        }
    }
   
    // This is the code for the transform calculation using the tracket headset as well.
    /*
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("space pressed");
            Matrix4x4 wTo = Matrix4x4.TRS(OptiCube.transform.position, OptiCube.transform.rotation, OptiCube.transform.localScale);
            //Matrix4x4 wTo_LH = convertRHtoLH(wTo); //uncommented for LH
            Debug.Log(" opticube Transform: \n" + wTo.ToString());
            Matrix4x4 wTh = Matrix4x4.TRS(HoloLens.transform.position, HoloLens.transform.rotation, HoloLens.transform.localScale);
            //Matrix4x4 wTh_LH = convertRHtoLH(wTo); //uncommented for LH
            Matrix4x4 cTg = Matrix4x4.TRS(GuideCube.transform.position, GuideCube.transform.rotation, GuideCube.transform.localScale);
            //cTh = cTg * (wTh_LH.inverse * wTo_LH).inverse; //uncommented for LH
            cTh = cTg * (wTh.inverse * wTo).inverse; //commented for LH
            averageAquired = true;
            Debug.Log(" Transform: \n" + cTh.ToString());
        }
        if (averageAquired)
        {
            Matrix4x4 wTo = Matrix4x4.TRS(OptiCube.transform.position, OptiCube.transform.rotation, OptiCube.transform.localScale);
           // Matrix4x4 wTo_LH = convertRHtoLH(wTo); //uncommented for LH
            Matrix4x4 wTh = Matrix4x4.TRS(HoloLens.transform.position, HoloLens.transform.rotation, HoloLens.transform.localScale);
           // Matrix4x4 wTh_LH = convertRHtoLH(wTh); //uncommented for LH
            //cTo = cTh * wTh_LH.inverse * wTo_LH; //uncommented for LH
            cTo = cTh * wTh.inverse * wTo; //commented for LH
            newPosition = new Vector3(cTo.m03, cTo.m13, cTo.m23);
            newRotation = cTo.rotation;
            MatchCube.transform.localScale = scale;
            MatchCube.transform.SetPositionAndRotation(newPosition, newRotation);
        }
    }
    */
    public Matrix4x4 convertRHtoLH(Matrix4x4 RH)
    {
        col1.Set(RH.m00, -RH.m10, RH.m20, 0);
        col2.Set(-RH.m01, RH.m11, -RH.m21, 0);
        col3.Set(RH.m02, -RH.m12, RH.m22, 0);
        col4.Set(-RH.m03, RH.m13, -RH.m23, 1);
        Matrix4x4 LH = new Matrix4x4(col1, col2, col3, col4);
        return LH;
    }
}


