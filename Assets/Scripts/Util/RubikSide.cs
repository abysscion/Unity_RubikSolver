using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public class RubikSide : MonoBehaviour
    {
        public List<GameObject> boundCubes;
        public List<RColor> faces;
        public Vector3 orientation;
    
        [SerializeField] private GameObject rubikCenter;

        public void UpdateSideData()
        {
            orientation = (transform.position - rubikCenter.transform.position).normalized;
            var nearCubesVectors = GetNewNearCubesVectors();
            var partsMask = LayerMask.GetMask("CubicParts");
            var facesMask = LayerMask.GetMask("CubicFaces");

            boundCubes.Clear();
            foreach (var destination in nearCubesVectors)
            {
                if (Physics.Raycast(transform.position, destination, out var hitInfo, transform.localScale.x, partsMask))
                    boundCubes.Add(hitInfo.collider.gameObject);
            }
            boundCubes.Insert(4, gameObject);

            faces.Clear();
            foreach (var cube in boundCubes)
            {
                if (Physics.Raycast(cube.transform.position, orientation, out var hitInfo, transform.localScale.x, facesMask))
                    faces.Add(Tools.GetRColorByTag(hitInfo.collider.tag));
            }
        }

        private IEnumerable<Vector3> GetNewNearCubesVectors()
        {
            var right = orientation == transform.right || orientation == -transform.right
                ? -transform.forward
                : transform.right;
            var up = Vector3.Cross(orientation, right);
            // return new List<Vector3> {right, -right, up, -up, up + right, up - right, -up - right, -up + right};
            return new List<Vector3> {up + right, up, up - right, right, -right, -up + right, -up, -up - right};
        }

        private void Awake()
        {
            if (!rubikCenter)
                rubikCenter = GameObject.Find("CenterCube");
            boundCubes = new List<GameObject>();
            faces = new List<RColor>();
        
            UpdateSideData();
        }
    }
}
