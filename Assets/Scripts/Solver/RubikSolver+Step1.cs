using System.Collections.Generic;
using Util;

namespace Solver
{
    public partial class RubikSolver
    {
        //for step1 points of interests are
        /*
                 [0][ ][1]
                 [ ][ ][ ]
                 [3][ ][2]        
        [ ][ ][ ][ ][ ][ ][ ][ ][ ][ ][ ][ ]
        [ ][ ][ ][ ][ ][ ][ ][ ][ ][ ][ ][ ]
        [0][ ][3][3][ ][2][2][ ][1][1][ ][0]
                 [3][ ][2]
                 [ ][ ][ ]
                 [0][ ][1]        
        */
        
        public void SolveStep1()
        {
            if (Step1ConditionsMet())
                return;
            
            var lastRotationsCount = _rotationsMade.Count;
            var triplets = GetCurrentLowerTriplets();
            var cornersColors = new List<(sbyte, sbyte)>
            {
                (_lCenterColor, _fCenterColor),
                (_fCenterColor, _rCenterColor),
                (_rCenterColor, _bCenterColor),
                (_bCenterColor, _lCenterColor)
            };

            while (!Step1ConditionsMet())
            {
                var rotationsCount = _rotationsMade.Count;
                for (var i = 0; i < triplets.Count; i++)
                {
                    if (TripletAtCorrectPosition(triplets[i], cornersColors[i], _uCenterColor))
                    {
                        Step1MoveLowerTripletOnTop(triplets[i]);
                        triplets = GetCurrentLowerTriplets();
                        break;
                    }
                }
                if (rotationsCount == _rotationsMade.Count)
                {
                    if (Step1IsUpperTripletStuck())
                        Step1FixUpperTripletStuck();
                    else
                        PerformRotation(RSide.Down, RotationType.Clockwise);
                    triplets = GetCurrentLowerTriplets();
                }
            }
            
            RotationsCountStep1 = _rotationsMade.Count - lastRotationsCount;
        }

        private bool Step1ConditionsMet()
        {
            return _lFaces[0, 0].Color == _lFaces[1, 0].Color && _lFaces[1, 0].Color == _lFaces[2, 0].Color &&
                   _fFaces[0, 0].Color == _fFaces[1, 0].Color && _fFaces[1, 0].Color == _fFaces[2, 0].Color &&
                   _rFaces[0, 0].Color == _rFaces[1, 0].Color && _rFaces[1, 0].Color == _rFaces[2, 0].Color &&
                   _bFaces[0, 0].Color == _bFaces[1, 0].Color && _bFaces[1, 0].Color == _bFaces[2, 0].Color;
        }

        private bool Step1IsUpperTripletStuck()
        {
            //triplet is stuck if it appears on upper level, in this situation we'll met useless down side rotating
            //so if down side rotated 4 times in row without triplet picking it means that triplet stuck.

            return HappenedFourUselessRotationsInARaw();
        }

        private void Step1FixUpperTripletStuck()
        {
            //algorithm cant see triplet at upper level, so we should drop triplet at lower level
            //firstly we check wrong centerColor on edge side, then wrong triplet position on top
            if (_lFaces[0, 0].Color == _uCenterColor || _bFaces[2, 0].Color == _uCenterColor ||
                _uFaces[0, 0].Color == _uCenterColor && _lFaces[0, 0].Color != _lCenterColor)
            {
                PerformRotation(RSide.Left, RotationType.CounterClockwise);
                PerformRotation(RSide.Down, RotationType.CounterClockwise);
                PerformRotation(RSide.Left, RotationType.Clockwise);
            }
            else if (_fFaces[0, 0].Color == _uCenterColor || _lFaces[2, 0].Color == _uCenterColor ||
                     _uFaces[0, 2].Color == _uCenterColor && _fFaces[0, 0].Color != _fCenterColor)
            {
                PerformRotation(RSide.Front, RotationType.CounterClockwise);
                PerformRotation(RSide.Down, RotationType.CounterClockwise);
                PerformRotation(RSide.Front, RotationType.Clockwise);
            }
            else if (_rFaces[0, 0].Color == _uCenterColor || _fFaces[2, 0].Color == _uCenterColor ||
                     _uFaces[2, 2].Color == _uCenterColor && _rFaces[0, 0].Color != _rCenterColor)
            {
                PerformRotation(RSide.Right, RotationType.CounterClockwise);
                PerformRotation(RSide.Down, RotationType.CounterClockwise);
                PerformRotation(RSide.Right, RotationType.Clockwise);
            }
            else if (_bFaces[0, 0].Color == _uCenterColor || _rFaces[2, 0].Color == _uCenterColor ||
                     _uFaces[2, 0].Color == _uCenterColor && _bFaces[0, 0].Color != _bCenterColor)
            {
                PerformRotation(RSide.Back, RotationType.CounterClockwise);
                PerformRotation(RSide.Down, RotationType.CounterClockwise);
                PerformRotation(RSide.Back, RotationType.Clockwise);
            }
        }

        private void Step1MoveLowerTripletOnTop(SolverFace[] triplet)
        {
            SolverFace face = null;

            foreach (var tFace in triplet)
            {
                if (tFace.Color != _uCenterColor)
                    continue;
                face = tFace;
                break;
            }

            // 3 point  rotations
            if (face == _lFaces[2, 2])
            {
                PerformRotation(RSide.Left, RotationType.Clockwise);
                PerformRotation(RSide.Down, RotationType.Clockwise);
                PerformRotation(RSide.Left, RotationType.CounterClockwise);
            }
            
            if (face == _fFaces[0, 2])
            {
                PerformRotation(RSide.Front, RotationType.CounterClockwise);
                PerformRotation(RSide.Down, RotationType.CounterClockwise);
                PerformRotation(RSide.Front, RotationType.Clockwise);
            }
            
            if (face == _dFaces[0, 0])
            {
                PerformRotation(RSide.Front, RotationType.CounterClockwise);
                PerformRotation(RSide.Down, RotationType.Halfturn);
                PerformRotation(RSide.Front, RotationType.Clockwise);
                PerformRotation(RSide.Down, RotationType.Clockwise);
                PerformRotation(RSide.Front, RotationType.CounterClockwise);
                PerformRotation(RSide.Down, RotationType.CounterClockwise);
                PerformRotation(RSide.Front, RotationType.Clockwise);
            }
            //==================================================================
            
            // 2 point rotations
            if (face == _fFaces[2, 2])
            {
                PerformRotation(RSide.Front, RotationType.Clockwise);
                PerformRotation(RSide.Down, RotationType.Clockwise);
                PerformRotation(RSide.Front, RotationType.CounterClockwise);
            }
            
            if (face == _rFaces[0, 2])
            {
                PerformRotation(RSide.Right, RotationType.CounterClockwise);
                PerformRotation(RSide.Down, RotationType.CounterClockwise);
                PerformRotation(RSide.Right, RotationType.Clockwise);
            }
            
            if (face == _dFaces[2, 0])
            {
                PerformRotation(RSide.Right, RotationType.CounterClockwise);
                PerformRotation(RSide.Down, RotationType.Halfturn);
                PerformRotation(RSide.Right, RotationType.Clockwise);
                PerformRotation(RSide.Down, RotationType.Clockwise);
                PerformRotation(RSide.Right, RotationType.CounterClockwise);
                PerformRotation(RSide.Down, RotationType.CounterClockwise);
                PerformRotation(RSide.Right, RotationType.Clockwise);
            }
            //==================================================================
            
            // 1 point rotations
            if (face == _rFaces[2, 2])
            {
                PerformRotation(RSide.Right, RotationType.Clockwise);
                PerformRotation(RSide.Down, RotationType.Clockwise);
                PerformRotation(RSide.Right, RotationType.CounterClockwise);
            }
            
            if (face == _bFaces[0, 2])
            {
                PerformRotation(RSide.Back, RotationType.CounterClockwise);
                PerformRotation(RSide.Down, RotationType.CounterClockwise);
                PerformRotation(RSide.Back, RotationType.Clockwise);
            }
            
            if (face == _dFaces[2, 2])
            {
                PerformRotation(RSide.Back, RotationType.CounterClockwise);
                PerformRotation(RSide.Down, RotationType.Halfturn);
                PerformRotation(RSide.Back, RotationType.Clockwise);
                PerformRotation(RSide.Down, RotationType.Clockwise);
                PerformRotation(RSide.Back, RotationType.CounterClockwise);
                PerformRotation(RSide.Down, RotationType.CounterClockwise);
                PerformRotation(RSide.Back, RotationType.Clockwise);
            }
            //==================================================================
            
            // 1 point rotations
            if (face == _bFaces[2, 2])
            {
                PerformRotation(RSide.Back, RotationType.Clockwise);
                PerformRotation(RSide.Down, RotationType.Clockwise);
                PerformRotation(RSide.Back, RotationType.CounterClockwise);
            }
            
            if (face == _lFaces[0, 2])
            {
                PerformRotation(RSide.Left, RotationType.CounterClockwise);
                PerformRotation(RSide.Down, RotationType.CounterClockwise);
                PerformRotation(RSide.Left, RotationType.Clockwise);
            }
            
            if (face == _dFaces[0, 2])
            {
                PerformRotation(RSide.Left, RotationType.CounterClockwise);
                PerformRotation(RSide.Down, RotationType.Halfturn);
                PerformRotation(RSide.Left, RotationType.Clockwise);
                PerformRotation(RSide.Down, RotationType.Clockwise);
                PerformRotation(RSide.Left, RotationType.CounterClockwise);
                PerformRotation(RSide.Down, RotationType.CounterClockwise);
                PerformRotation(RSide.Left, RotationType.Clockwise);
            }
            //==================================================================
        }
    }
}