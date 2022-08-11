using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;

public class fillMatrix : MonoBehaviour
{

    // create a dense zero-vector of length 10

    List<Vector<double>> vecList = new List<Vector<double>>();


    // put all children Transforms in a Math.Net Vector list
    public List<Vector<double>> TransVecList()
    {
        foreach (Transform fidsTrans in this.GetComponentInChildren<Transform>())
        {
            Debug.Log(fidsTrans.gameObject.name.ToString());
            Vector<double> tmpVec = Vector<double>.Build.Dense(3);
            tmpVec[0] = fidsTrans.position.x;
            tmpVec[1] = fidsTrans.position.y;
            tmpVec[2] = fidsTrans.position.z;
            vecList.Add(tmpVec);
        }
        return vecList;
    }
}