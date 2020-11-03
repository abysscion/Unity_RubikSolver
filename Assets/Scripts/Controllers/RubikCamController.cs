using UnityEngine;

namespace Controllers
{
    public class RubikCamController : MonoBehaviour
    {
        [SerializeField] private Transform rubik = null;
        [SerializeField] private float rotationSpeed = 5;

        private void Update()
        {
            if (!Input.GetMouseButton(1))
                return;

            if (Input.GetAxis("Mouse X") < 0.01 || Input.GetAxis("Mouse X") > 0.01)
                transform.RotateAround(rubik.position, transform.up, Input.GetAxis("Mouse X") * rotationSpeed);
            if (Input.GetAxis("Mouse Y") < 0.01 || Input.GetAxis("Mouse Y") > 0.01)
                transform.RotateAround(rubik.position, transform.right, -Input.GetAxis("Mouse Y") * rotationSpeed);
        }
    }
}
