using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformerInputWithTool : MonoBehaviour
{
    public MeshDeformerWithTool deformableMesh;
    public float deformableRadius;

    private void Start()
    {

    }

    void Update ()
    {

    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {
            deformableMesh.UpdateVertex(contact.point, deformableRadius);
            Debug.Log("Collision Detected");
        }
    }
}
