using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using RSide = RubickSide.RSide;

public class SidesController : MonoBehaviour
{
    public bool resetSidesRequired;
    public bool updateRequired;
    public bool isAnyRotating;
    
    [SerializeField] private RotateSpeeds sideRotateSpeed = RotateSpeeds.Small;
    [SerializeField] private float shuffleStepsMax = 10;
    [SerializeField] private float shuffleStepsMin = 5;
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
    
    public void AddRotationToQueue((RotationType, RSide) command)
    {
        _rotationCommands.Enqueue(command);
    }

    public void StopRotating()
    {
        _rotationCommands.Clear();
    }

    public void ShuffleSides()
    {
        var typesCount = Enum.GetNames(typeof(RotationType)).Length;
        var sidesCount = Enum.GetNames(typeof(RSide)).Length;
        
        for (var i = 0; i < Random.Range(shuffleStepsMin, shuffleStepsMax); i++)
            AddRotationToQueue(((RotationType) Random.Range(0, typesCount), (RSide) Random.Range(0, sidesCount)));
    }

    private void Start()
    {
        _rubickStartStateData = new Dictionary<GameObject, (Vector3, Quaternion)>();
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
        updateRequired = true;
        SaveRubickStartState();
    }

    private void FixedUpdate()
    {
        UpdateSidesData();
        ProceedRotationStep();
        ExecuteNextRotationCommand();
        ResetSides();
    }
    
    private void UpdateSidesData()
    {
        if (!updateRequired) return;
        
        foreach (var side in _sides)
        {
            side.UpdateSideData();
            if (side.boundCubes.Count < 9)
                Debug.LogWarning(side.gameObject.name + " has less than 9 cubes.");
        }

        updateRequired = false;
    }

    private void ResetSides()
    {
        if (isAnyRotating || !resetSidesRequired) return;
        
        foreach (var side in _sides)
        {
            foreach (var cube in side.boundCubes)
            {
                cube.transform.position = _rubickStartStateData[cube].pos;
                cube.transform.rotation = _rubickStartStateData[cube].rot;
            }
        }

        resetSidesRequired = false;
        updateRequired = true;
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
            updateRequired = true;
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
