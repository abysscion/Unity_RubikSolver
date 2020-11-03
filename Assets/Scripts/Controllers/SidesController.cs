using System;
using System.Collections.Generic;
using Solver;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace Controllers
{
    public class SidesController : MonoBehaviour
    {
        [HideInInspector] public bool resetSidesRequired;
        [HideInInspector] public bool updateRequired;
        [HideInInspector] public bool isAnyRotating;
        
        [SerializeField] private RotateSpeeds sideRotateSpeed = RotateSpeeds.Small;
        [SerializeField] private float shuffleStepsMax = 10;
        [SerializeField] private float shuffleStepsMin = 5;
        private Dictionary<GameObject, (Vector3 pos, Quaternion rot)> _rubickStartStateData;
        private Queue<(RotationType, RSide)> _rotationCommands;
        private List<RubikSide> _sides;
        private RubikSide _currentRotSide;
        private RubikSide _selectedSide;
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
            _rotationCommands.Clear();
            var typesCount = Enum.GetNames(typeof(RotationType)).Length;
            var sidesCount = Enum.GetNames(typeof(RSide)).Length;
        
            for (var i = 0; i < Random.Range(shuffleStepsMin, shuffleStepsMax); i++)
                AddRotationToQueue(((RotationType) Random.Range(0, typesCount), (RSide) Random.Range(0, sidesCount)));
        }

        public SolverSide[] GetSolverSides()
        {
            var sidesArr = new SolverSide[6];
            var facesMask = LayerMask.GetMask("CubicFaces");
            
            var mapperSides = GameObject.FindGameObjectsWithTag("MapperSides"); // GetComponentsInChildren<Transform>();
            var mapperSidesList = new List<GameObject> {null, null, null, null, null, null};
            for (var i = 0; i < 6; i++)
            {
                if (mapperSides[i].name.Equals("Front"))
                    mapperSidesList[0] = mapperSides[i];
                else if (mapperSides[i].name.Equals("Left"))
                    mapperSidesList[1] = mapperSides[i];
                else if (mapperSides[i].name.Equals("Right"))
                    mapperSidesList[2] = mapperSides[i];
                else if (mapperSides[i].name.Equals("Down"))
                    mapperSidesList[3] = mapperSides[i];
                else if (mapperSides[i].name.Equals("Up"))
                    mapperSidesList[4] = mapperSides[i];
                else if (mapperSides[i].name.Equals("Back"))
                    mapperSidesList[5] = mapperSides[i];
            }
            mapperSidesList.CopyTo(mapperSides);

            for (var i = 0; i < mapperSides.Length; i++)
            {
                sidesArr[i] = new SolverSide(RColor.Red);

                var mapperSideFaces = mapperSides[i].GetComponentsInChildren<Transform>();
                var mapperSideFacesList = new List<Transform> {null, null, null, null, null, null, null, null, null};
                for (var x = 1; x < mapperSideFaces.Length; x++)
                    mapperSideFacesList[int.Parse(mapperSideFaces[x].name) - 1] = mapperSideFaces[x];
                mapperSideFacesList.CopyTo(mapperSideFaces, 1);

                for (var j = 1; j < mapperSideFaces.Length; j++)
                {
                    if (Physics.Raycast(mapperSideFaces[j].position, mapperSideFaces[j].forward, out var hitInfo,
                        transform.localScale.x, facesMask))
                        sidesArr[i].Faces[(j - 1) % 3, (j - 1) / 3].Color = (sbyte) Tools.GetRColorByTag(hitInfo.collider.tag);
                }
            }

            return sidesArr;
        }
        
        private void Start()
        {
            _rubickStartStateData = new Dictionary<GameObject, (Vector3, Quaternion)>();
            _rotationCommands = new Queue<(RotationType, RSide)>();
            _sides = new List<RubikSide>
            {
                GameObject.Find("CenterRed").GetComponent<RubikSide>(),        //Front -- 0
                GameObject.Find("CenterBlue").GetComponent<RubikSide>(),       //Left -- 1
                GameObject.Find("CenterGreen").GetComponent<RubikSide>(),      //Right -- 2
                GameObject.Find("CenterWhite").GetComponent<RubikSide>(),      //Down -- 3
                GameObject.Find("CenterYellow").GetComponent<RubikSide>(),     //Up -- 4
                GameObject.Find("CenterOrange").GetComponent<RubikSide>()      //Back -- 5
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
            if (_rotationCommands == null)
                return;
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

        private void OnDrawGizmos()
        {
            if (_sides == null) return;
            if (_sides.Count <= 0) return;
        
            Gizmos.DrawIcon(_sides[(int)RSide.Front].orientation * 2f, "F.tiff");
            Gizmos.DrawIcon(_sides[(int)RSide.Left].orientation * 2f, "L.tiff");
            Gizmos.DrawIcon(_sides[(int)RSide.Up].orientation * 2f, "U.tiff");
            Gizmos.DrawIcon(_sides[(int)RSide.Back].orientation * 2f, "B.tiff");
            Gizmos.DrawIcon(_sides[(int)RSide.Right].orientation * 2f, "R.tiff");
            Gizmos.DrawIcon(_sides[(int)RSide.Down].orientation * 2f, "D.tiff");
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
}
