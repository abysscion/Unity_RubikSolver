using UnityEngine;

public class WholeRubickController : MonoBehaviour
{
    [SerializeField] private float zoomFactor = 2f;
    [SerializeField] private float rotateSpeed = 1f;
    private readonly Vector3 _minZoom = new Vector3(-38f, 30.9f, -38f);
    private readonly Vector3 _maxZoom = new Vector3(-6f, 4.8f, -6f);
    private SidesController _sController;
    private Transform _camTrans;

    private void Start()
    {
        _camTrans = Camera.main.transform;
        _sController = GetComponent<SidesController>();
    }

    private void Update()
    {
        HandleRotate();
        HandleZoom();
    }

    private void HandleRotate()
    {
        if (Input.GetMouseButton(0) || _sController.isAnyRotating) return;
        if (Input.GetMouseButtonUp(1))
            _sController.UpdateSidesData();
        if (!Input.GetMouseButton(1)) return;

        var horizontalVec = Vector3.right - Vector3.forward;
        var rotX = Input.GetAxis("Mouse X") * rotateSpeed;
        var rotY = Input.GetAxis("Mouse Y") * rotateSpeed;

        transform.Rotate(Vector3.up, -rotX, Space.World);
        transform.Rotate(horizontalVec, rotY, Space.World);
    }

    private void HandleZoom()
    {
        if (Mathf.Abs(Input.mouseScrollDelta.y) <= 0) return;
        
        _camTrans.Translate(_camTrans.forward * (Input.mouseScrollDelta.y * zoomFactor), Space.World);
        var camPos = _camTrans.position;
        if (camPos.x < _minZoom.x || camPos.y > _minZoom.y || camPos.z < _minZoom.z)
            _camTrans.position = _minZoom;
        else if (camPos.x > _maxZoom.x || camPos.y < _maxZoom.y || camPos.z > _maxZoom.z)
            _camTrans.position = _maxZoom;
    }
}
