using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformerWithTool : MonoBehaviour
{
    Mesh deformingMesh;
    Vector3[] originalVertices;
    Vector3[] displacedVertices;
    public float maximumDepression = 0.1f;
    
    private void Start()
    {
        MeshCollider collider = GetComponent<MeshCollider>();
        collider.enabled = true;
        deformingMesh = GetComponent<MeshFilter>().mesh;
        originalVertices = deformingMesh.vertices;
        displacedVertices = new Vector3[originalVertices.Length];
        for (int i = 0; i <originalVertices.Length; i++)
        {
            displacedVertices[i] = originalVertices[i];
        }
    }

    private void Update()
    {
       deformingMesh.vertices = displacedVertices;
       deformingMesh.RecalculateNormals();
    }

    public void UpdateVertex (Vector3 depressionPoint, float radius)
    {
        var worldPos4 = this.transform.worldToLocalMatrix.MultiplyPoint(depressionPoint); //var worldPos4 = this.transform.worldToLocalMatrix * depressionPoint;
        var worldPos = new Vector3(worldPos4.x, worldPos4.y, worldPos4.z);

        for (int i = 0; i<displacedVertices.Length; i++)
        {
            var distance = (worldPos - displacedVertices[i]).magnitude;
            //var distance = ((displacedVertices[i] - depressionPoint).magnitude);
            if (distance < radius)
            {
                //displacedVertices[i] = displacedVertices[i] - originalVertices[i] *maximumDepression;
                displacedVertices[i] -= maximumDepression * displacedVertices[i];
                Debug.Log("Mesh Depressed");
             }
        }
        

    }
}
