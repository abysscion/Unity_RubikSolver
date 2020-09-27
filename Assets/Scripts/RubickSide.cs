using System.Collections.Generic;
using UnityEngine;

public class RubickSide : MonoBehaviour
{
    public List<GameObject> boundCubes;
    public List<RColor> faces;
    public GameObject rubickCenter;
    public Vector3 orientation;
    
    public enum RSide
    {
        Front = 0,
        Left = 1,
        Right = 2,
        Down = 3,
        Up = 4,
        Back = 5
    }

    public void UpdateSideData()
    {
        orientation = (transform.position - rubickCenter.transform.position).normalized;
        var nearCubesVectors = GetNewNearCubesVectors();
        var partsMask = LayerMask.GetMask("CubicParts");
        var facesMask = LayerMask.GetMask("CubicFaces");

        boundCubes.Clear();
        boundCubes.Add(gameObject);
        foreach (var destination in nearCubesVectors)
        {
            if (Physics.Raycast(transform.position, destination, out var hitInfo, transform.localScale.x, partsMask))
                boundCubes.Add(hitInfo.collider.gameObject);
        }

        faces.Clear();
        foreach (var cube in boundCubes)
        {
            if (Physics.Raycast(cube.transform.position, orientation, out var hitInfo, transform.localScale.x, facesMask))
                faces.Add(Util.GetRColorByTag(hitInfo.collider.tag));
        }
    }

    private IEnumerable<Vector3> GetNewNearCubesVectors()
    {
        var right = orientation == transform.right || orientation == -transform.right
            ? -transform.forward
            : transform.right;
        var up = Vector3.Cross(orientation, right);
        return new List<Vector3> {right, -right, up, -up, up + right, up - right, -up - right, -up + right};
    }

    private void Awake()
    {
        if (!rubickCenter)
            rubickCenter = GameObject.Find("CenterCube");
        boundCubes = new List<GameObject>();
        faces = new List<RColor>();
        
        UpdateSideData();
    }
}
