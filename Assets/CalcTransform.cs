using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using Microsoft.MixedReality.Toolkit;

public class CalcTransform : MonoBehaviour
{
    private Matrix<double> covMatrix = Matrix<double>.Build.Dense(3, 3);
    private List<Vector<double>> vecListSrc = new List<Vector<double>>();
    private List<Vector<double>> vecListSrcCoM = new List<Vector<double>>();
    private List<Vector<double>> vecListDst = new List<Vector<double>>();
    private List<Vector<double>> vecListDstCoM = new List<Vector<double>>();

    public GameObject srcContainer, dstContainer;

    public bool calibrated = false;
    public bool frameByFrame;






    //public GameObject testFidSrc, testFidDst;

    // Use this for initialization
    void Start()
    {




    }

    private void Update()
    {
       /* if (Input.GetKeyDown("space") )
        {*/
            if (Input.GetKeyDown("space"))
            {
                calibrated = true;
            }
            dstContainer.transform.position = Vector3.zero;
            dstContainer.transform.rotation = Quaternion.identity;
            vecListSrc = srcContainer.GetComponent<fillMatrix>().TransVecList();
            vecListDst = dstContainer.GetComponent<fillMatrix>().TransVecList();
            Debug.Log(vecListSrc);

            // calculate center of mass of Vector list
            Vector<double> coMSrc = CalcCoM(vecListSrc);
            Vector<double> coMDst = CalcCoM(vecListDst);

            // for debugging only
            // CreateMarker(new Vector3((float)coMSrc[0], (float)coMSrc[1], (float)coMSrc[2]), srcContainer);
            // CreateMarker(new Vector3((float)coMDst[0], (float)coMDst[1], (float)coMDst[2]), dstContainer);

            // calculate vectors translated to center of mass
            vecListSrcCoM = CalcCoMVecs(vecListSrc, coMSrc);
            vecListDstCoM = CalcCoMVecs(vecListDst, coMDst);

            // for debugging only
            //foreach (Vector<double> vec in vecListSrcCoM)
            //{
            //    CreateMarker(new Vector3((float)vec[0], (float)vec[1], (float)vec[2]), srcContainer);
            //}
            //foreach (Vector<double> vec in vecListDstCoM)
            //{
            //    CreateMarker(new Vector3((float)vec[0], (float)vec[1], (float)vec[2]), dstContainer);
            //}


            // calculate covariance matrix
            covMatrix = CalcCov(vecListSrcCoM, vecListDstCoM);

            // perform singular value decomposition
            var svd = covMatrix.Svd(true);

            // get the rotation through R = U VT
            // /good resource : https://igl.ethz.ch/projects/ARAP/svd_rot.pdf
            // /online documentation designates U as V and V as U ? ---- http://nghiaho.com/?page_id=671

            Debug.Log("U" + svd.U.ToMatrixString());
            var rot1 = svd.U * svd.VT;
            var tra1 = coMSrc - rot1 * coMDst;




            var rot = GetRot(rot1, tra1, svd);
            Debug.Log("here " + rot.ToMatrixString());
            var tra = coMSrc - rot * coMDst;






            // convert Math rotation and translation to Matrix4x4
            Quaternion rotMat = QuaternionFromMatrix(MathRot2Mat4x4(rot, tra));


            // apply rotation and transformation to src gameobject
            //RotandTrans(testFidSrc, testFidDst, rot, tra);
            

           RotandTransRigid(srcContainer, dstContainer, rotMat, rot, tra); 

            Debug.Log("end");

       /* }*/

    }
    Matrix<double> GetRot(Matrix<double> rotdd, Vector<double> tradd, Svd<double> svd1)
    {
        //check for reflection
      
        if (MathRot2Mat4x4(rotdd, tradd).determinant > 0)
        {
            Debug.Log("Reflection = false");

        }
        else if (MathRot2Mat4x4(rotdd, tradd).determinant < 0)
        {
            Debug.Log("Reflection = true");

        }
        return MathRot2Mat4x4(rotdd, tradd).determinant > 0 ? svd1.U * svd1.VT : GetSVT(svd1.U) * svd1.VT;
    }
    Matrix<double> GetSVT(Matrix<double> c)
    {
        Matrix<double> mat5 = c;
        Debug.Log("c " + c.ToMatrixString());
        for (int ir = 0; ir < mat5.RowCount; ir++) mat5[ir, 2] = -1 * mat5[ir, 2];

        Debug.Log("mat5 " + mat5.ToMatrixString());
        return mat5;
    }



    // calculate vectors translated to center of mass
    List<Vector<double>> CalcCoMVecs(List<Vector<double>> fids, Vector<double> coM)
    {
        List<Vector<double>> vecListCoM = new List<Vector<double>>();
        foreach (Vector<double> vec in fids)
        {
            vecListCoM.Add(vec - coM);
        }
        return vecListCoM;
    }

    // calculate covariance matrix
    Matrix<double> CalcCov(List<Vector<double>> srcVecs, List<Vector<double>> dstVecs)
    {
        for (int i = 0; i < srcVecs.Count; i++)
        {
            covMatrix += ColVec2Mat(srcVecs[i]) * RowVec2Mat(dstVecs[i]);
        }
        return covMatrix;
    }

    void RotandTransRigid(GameObject srcObj, GameObject dstObj, Quaternion rotMat4, Matrix<double> rot, Vector<double> tra)
    {
        Vector<double> srcObjPos = Vector<double>.Build.Dense(3);
        srcObjPos[0] = srcObj.transform.position.x;
        srcObjPos[1] = srcObj.transform.position.y;
        srcObjPos[2] = srcObj.transform.position.z;
        Vector<double> dstObjPos = Vector<double>.Build.Dense(3);
        dstObjPos[0] = dstObj.transform.position.x;
        dstObjPos[1] = dstObj.transform.position.y;
        dstObjPos[2] = dstObj.transform.position.z;

        // All 
        Vector<double> transVec = rot * dstObjPos + tra;
        //Matrix<double> transVecM = rot * ColVec2Mat(dstObjPos) + ColVec2Mat(tra);
        //Matrix<double> transVecMT = rot * RowVec2Mat(dstObjPos) + ColVec2Mat(tra);
        //Matrix<double> transVecMb = rot * ColVec2Mat(dstObjPos) + RowVec2Mat(tra);
        //Matrix<double> transVecMbT = rot * RowVec2Mat(dstObjPos) + RowVec2Mat(tra);
        Vector<double> newObjPos = transVec;
        if (!double.IsNaN(newObjPos[0]) && !double.IsNaN(newObjPos[0]) && !double.IsNaN(newObjPos[0])){
            dstObj.transform.position = new Vector3((float)newObjPos[0], (float)newObjPos[1], (float)newObjPos[2]);
        }
       
        dstObj.transform.rotation = rotMat4;
    }


    void RotandTrans(GameObject srcObj, GameObject dstObj, Matrix<double> rot, Vector<double> tra)
    {
        Vector<double> srcObjPos = Vector<double>.Build.Dense(3);
        srcObjPos[0] = srcObj.transform.position.x;
        srcObjPos[1] = srcObj.transform.position.y;
        srcObjPos[2] = srcObj.transform.position.z;
        Vector<double> dstObjPos = Vector<double>.Build.Dense(3);
        dstObjPos[0] = dstObj.transform.position.x;
        dstObjPos[1] = dstObj.transform.position.y;
        dstObjPos[2] = dstObj.transform.position.z;

        // All 
        Vector<double> transVec = rot * dstObjPos + tra;
        Vector<double> newObjPos = transVec;
        dstObj.transform.position = new Vector3((float)newObjPos[0], (float)newObjPos[1], (float)newObjPos[2]);
    }

    // create spherical marker
    void CreateMarker(Vector3 pos, GameObject parentObj)
    {
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere); // make sphere
        sphere.transform.parent = parentObj.transform; // !!! set parent before changing position, otherwise collider error! Unity 2018.1.9 BUG
        sphere.transform.localScale = Vector3.one * 0.1f; // scale sphere down
        sphere.transform.position = pos; // position sphere at cursor position
    }

    //// static functions
    ///     // convert column vector to matrix that has the vector in col 1 and 0 everywhere else
    static Matrix<double> ColVec2Mat(Vector<double> vec)
    {
        Matrix<double> colVecMatrix = Matrix<double>.Build.Dense(3, 3);
        colVecMatrix[0, 0] = vec[0];
        colVecMatrix[1, 0] = vec[1];
        colVecMatrix[2, 0] = vec[2];
        return colVecMatrix;
    }

    // convert row vector to matrix that has the vector in row 1 and 0 everywhere else
    static Matrix<double> RowVec2Mat(Vector<double> vec)
    {
        Matrix<double> rowVecMatrix = Matrix<double>.Build.Dense(3, 3);
        rowVecMatrix[0, 0] = vec[0];
        rowVecMatrix[0, 1] = vec[1];
        rowVecMatrix[0, 2] = vec[2];
        return rowVecMatrix;
    }

    static Quaternion QuaternionFromMatrix(Matrix4x4 m)
    {
        // Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
        Quaternion q = new Quaternion();
        q.w = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) / 2;
        q.x = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) / 2;
        q.y = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] + m[1, 1] - m[2, 2])) / 2;
        q.z = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] - m[1, 1] + m[2, 2])) / 2;
        q.x *= Mathf.Sign(q.x * (m[2, 1] - m[1, 2]));
        q.y *= Mathf.Sign(q.y * (m[0, 2] - m[2, 0]));
        q.z *= Mathf.Sign(q.z * (m[1, 0] - m[0, 1]));
        return q;
    }

    //public static Quaternion QuaternionFromMatrix(Matrix4x4 m) 
    //{ 
    //    return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1)); 
    //}

    // calculate center of mass of Vector list
    static Vector<double> CalcCoM(List<Vector<double>> Fids)
    {
        Vector<double> coM = Vector<double>.Build.Dense(3);
        foreach (Vector<double> vec in Fids)

            coM += vec;

        coM = coM.Divide(Fids.Count);
        return coM;
    }

    static Matrix4x4 MathRot2Mat4x4(Matrix<double> rotmat, Vector<double> travec)
    {
        Matrix4x4 mat4 = new Matrix4x4();
        mat4.m00 = (float)rotmat[0, 0];
        mat4.m01 = (float)rotmat[0, 1];
        mat4.m02 = (float)rotmat[0, 2];
        mat4.m03 = (float)travec[0];
        mat4.m10 = (float)rotmat[1, 0];
        mat4.m11 = (float)rotmat[1, 1];
        mat4.m12 = (float)rotmat[1, 2];
        mat4.m13 = (float)travec[1];
        mat4.m20 = (float)rotmat[2, 0];
        mat4.m21 = (float)rotmat[2, 1];
        mat4.m22 = (float)rotmat[2, 2];
        mat4.m23 = (float)travec[2];
        mat4.m30 = 0;
        mat4.m31 = 0;
        mat4.m32 = 0;
        mat4.m33 = 1;

        Debug.Log("mat4 " + mat4.ToString());
        Debug.Log("rotmat " + rotmat.ToMatrixString());
        Debug.Log("det " + mat4.determinant.ToString());
        //mat4.SetColumn(3, new Vector4(mat4.m02*1, mat4.m12 * -1, mat4.m21 * -1, mat4.m32));
        Debug.Log("det2 " + mat4.determinant.ToString());
        return mat4;

    }





}