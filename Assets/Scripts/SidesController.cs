using System.Collections.Generic;
using UnityEngine;
using RSide = RubickSide.RSide;

public class SidesController : MonoBehaviour
{
    public bool isAnyRotating;
    
    [SerializeField] private RotateSpeeds sideRotateSpeed = RotateSpeeds.Small;
    private Dictionary<GameObject, (Vector3 pos, Quaternion rot)> _rubickStartStateData;
    private Queue<(RotationType, RSide)> _rotationCommands;
    private List<RubickSide> _sides;
    private RubickSide _currentRotSide;
    private RubickSide _selectedSide;
    private Vector3 _mousePosOnClick;
    private float _targetAngle;
    private bool _isRotationClockwise;
    private int _rotationStepsRemain;

    private enum RotateSpeeds
    {
        Small = 1,
        Medium = 3,
        High = 6,
        Insta = 9
    }
    
    public void UpdateSidesData()
    {
        foreach (var side in _sides)
        {
            side.UpdateSideData();
            if (side.boundCubes.Count < 9)
                Debug.LogWarning(side.gameObject.name + " has less than 9 cubes.");
        }
    }
    
    public void AddRotationToQueue((RotationType rotType, RSide side) command)
    {
        _rotationCommands.Enqueue(command);
    }
    
    public void ResetRubick()
    {
        if (isAnyRotating) return;
        
        foreach (var side in _sides)
        {
            foreach (var cube in side.boundCubes)
            {
                cube.transform.position = _rubickStartStateData[cube].pos;
                cube.transform.rotation = _rubickStartStateData[cube].rot;
            }
        }
        UpdateSidesData();
    }

    private void Start()
    {
        _rubickStartStateData = new Dictionary<GameObject, (Vector3 pos, Quaternion rot)>();
        _rotationCommands = new Queue<(RotationType, RSide)>();
        _sides = new List<RubickSide>
        {
            GameObject.Find("CenterRed").GetComponent<RubickSide>(),        //Front -- 0
            GameObject.Find("CenterBlue").GetComponent<RubickSide>(),       //Left -- 1
            GameObject.Find("CenterGreen").GetComponent<RubickSide>(),      //Right -- 2
            GameObject.Find("CenterWhite").GetComponent<RubickSide>(),      //Down -- 3
            GameObject.Find("CenterYellow").GetComponent<RubickSide>(),     //Up -- 4
            GameObject.Find("CenterOrange").GetComponent<RubickSide>()      //Back -- 5
        };
        SaveRubickStartState();
        UpdateSidesData();
    }

    private void Update()
    { 
        // if (Input.GetKeyDown(KeyCode.Space))
            // ResetRubick();
    }
    
    private void FixedUpdate()
    {
        ProceedRotationStep();
        ExecuteNextRotationCommand();
    }

    private void ExecuteNextRotationCommand()
    {
        if (isAnyRotating || _rotationCommands.Count == 0)
            return;
        
        RotateSide(_rotationCommands.Dequeue());
    }

    private void RotateSide((RotationType, RSide) command)
    {
        if (isAnyRotating) return;

        var (type, side) = command;
        var turnAngle = type == RotationType.Halfturn ? 180 : 90;
        _currentRotSide = _sides[(int)side];
        _isRotationClockwise = type != RotationType.CounterClockwise;
        _rotationStepsRemain = turnAngle / (5 * (int) sideRotateSpeed);
        isAnyRotating = true;
    }

    private void ProceedRotationStep()
    {
        if (!isAnyRotating)
            return;
        if (_rotationStepsRemain > 0)
        {
            _rotationStepsRemain--;
            foreach (var cube in _currentRotSide.boundCubes)
            {
                cube.transform.RotateAround(_currentRotSide.gameObject.transform.position, _currentRotSide.orientation,
                    5 * (int) sideRotateSpeed * (_isRotationClockwise ? 1 : -1));
            }
        }
        else
        {
            isAnyRotating = false;
            _currentRotSide = null;
            UpdateSidesData();
        }
    }

    private void SaveRubickStartState()
    {
        if (isAnyRotating) return;
        
        _rubickStartStateData.Clear();
        foreach (var side in _sides)
        {
            foreach (var cube in side.boundCubes)
            {
                if (!_rubickStartStateData.ContainsKey(cube))
                    _rubickStartStateData.Add(cube, (cube.transform.position, cube.transform.rotation));
            }
        }
    }

    
    //TODO: someday it could be made
    /*
    private void HandleSideDrag()
    {
        if (Input.GetMouseButton(1)) return;
        if (Input.GetMouseButtonDown(0) && !_isAnyRotating)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var mask = LayerMask.GetMask("MouseClickForDrag");
            if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, mask))
            {
                _selectedSide = hitInfo.transform.parent.GetComponent<RubickSide>();
                _mousePosOnClick = Input.mousePosition;
                //=======================================================================
                // var sideIndex = sides.IndexOf(_selectedSide);
                // RotateSide((RSide)sideIndex, true);
                // Debug.Log("Side clicked: " + (RSide)sideIndex);
                //=======================================================================
            }
        }

        if (Input.GetMouseButton(0) && _selectedSide)
        {
            var mouseMove = Input.mousePosition - _mousePosOnClick;
            var rotAngle = mouseMove.x / 100; //rot speed
            RotateSideByDegree(_selectedSide, rotAngle);
            // Debug.Log(mouseMove.magnitude);
        }

        if (Input.GetMouseButtonUp(0))
        {
            var sRot = _selectedSide.boundCubes[0].transform.localRotation.eulerAngles;
            // local rot > check excess rotation > rotate by -angle
            var modVec = new Vector3(sRot.x % 90, sRot.y % 90, sRot.z % 90); //instead of 90 should 
            if (modVec.x != 0 || modVec.y != 0 || modVec.z != 0)
            {
                var angle = Mathf.Abs(modVec.x) > Mathf.Abs(modVec.y) ? modVec.x : modVec.y;
                angle = angle > Mathf.Abs(modVec.z) ? angle : modVec.z;
                Debug.Log("rotating by " + angle);
                RotateSideByDegree(_selectedSide, angle);
            }
            _selectedSide = null;
            Debug.Log("STARTED UPDATE");
            UpdateSidesData();
        }
    }
    
    private void RotateSideByDegree(RubickSide target, float angle)
    {
        var rotSide = target;
        foreach (var cube in rotSide.boundCubes)
            cube.transform.RotateAround(rotSide.gameObject.transform.position, rotSide.orientation, angle);
    }
    */
}
