using System;
using System.Collections.Generic;
using Util;

namespace Solver
{
    public partial class RubikSolver
    {
        //for step4 points of interests are
        /*
                 [ ][ ][ ]
                 [ ][ ][ ]
                 [ ][ ][ ]        
        [ ][ ][ ][ ][ ][ ][ ][ ][ ][ ][ ][ ]
        [ ][ ][ ][ ][ ][ ][ ][ ][ ][ ][ ][ ]
        [3][ ][0][0][ ][1][1][ ][2][2][ ][3]
                 [0][ ][1]
                 [ ][ ][ ]
                 [3][ ][2]        
        */

        public void SolveStep4()
        {
            const RSide rMimic = RSide.Left;
            const RSide dMimic = RSide.Up;
            const RSide uMimic = RSide.Down;
            var triplets = GetCurrentLowerTriplets();
            var lastRotationsCount = _rotationsMade.Count;

            if (!Step4TripletsAtCorrectCorners(triplets) && _rotationsMade.Count < 250)
            {
                var correctRightSide = Step4GetCorrectRightSide(ref triplets);
                var correctLeftSide = Step4GetOppositeForSide(correctRightSide);

                while (!Step4TripletsAtCorrectCorners(triplets))
                    Step4PerformCornersSwapAlgorithm(correctLeftSide, correctRightSide);
            }

            while (!Step4IsRubikComplete())
            {
                if (_dFaces[0, 0].Color == _dCenterColor)
                    PerformRotation(uMimic, RotationType.Clockwise);
                else
                {
                    PerformRotation(rMimic, RotationType.CounterClockwise);
                    PerformRotation(dMimic, RotationType.CounterClockwise);
                    PerformRotation(rMimic, RotationType.Clockwise);
                    PerformRotation(dMimic, RotationType.Clockwise);
                }
            }

            if (_lFaces[0, 2].Color == _fCenterColor)
                PerformRotation(uMimic, RotationType.Clockwise);
            else if (_lFaces[0, 2].Color == _bCenterColor)
                PerformRotation(uMimic, RotationType.CounterClockwise);
            else if (_lFaces[0, 2].Color == _rCenterColor)
                PerformRotation(uMimic, RotationType.Halfturn);

            if (_lFaces[1, 2].Color != _lCenterColor)
                return;
            
            RotationsCountStep4 = _rotationsMade.Count - lastRotationsCount;
        }

        private bool Step4IsRubikComplete()
        {
            if (_dFaces[0, 0].Color != _dCenterColor || _dFaces[2, 0].Color != _dCenterColor ||
                _dFaces[0, 2].Color != _dCenterColor || _dFaces[2, 2].Color != _dCenterColor)
                return false;
            // if (_lFaces[0, 2].Color != _lCenterColor || _lFaces[2, 2].Color != _lCenterColor)
            //     return false;
            // if (_fFaces[0, 2].Color != _lCenterColor || _fFaces[2, 2].Color != _lCenterColor)
            //     return false;
            // if (_rFaces[0, 2].Color != _lCenterColor || _rFaces[2, 2].Color != _lCenterColor)
            //     return false;
            // if (_bFaces[0, 2].Color != _lCenterColor || _bFaces[2, 2].Color != _lCenterColor)
            //     return false;
            return true;
        }
        
        private void Step4PerformCornersSwapAlgorithm(RSide lMimic, RSide rMimic)
        {
            // U R U' L' U R' U' L
            // instead of up we are using down, because we won't turn rubik upside down programmatically

            var uMimic = RSide.Down;
            
            PerformRotation(uMimic, RotationType.Clockwise);
            PerformRotation(rMimic, RotationType.Clockwise);
            PerformRotation(uMimic, RotationType.CounterClockwise);
            PerformRotation(lMimic, RotationType.CounterClockwise);
            PerformRotation(uMimic, RotationType.Clockwise);
            PerformRotation(rMimic, RotationType.CounterClockwise);
            PerformRotation(uMimic, RotationType.CounterClockwise);
            PerformRotation(lMimic, RotationType.Clockwise);
        }

        private RSide Step4GetOppositeForSide(RSide side)
        {
            switch (side)
            {
                case RSide.Front:
                    return RSide.Back;
                case RSide.Left:
                    return RSide.Right;
                case RSide.Right:
                    return RSide.Left;
                case RSide.Back:
                    return RSide.Front;
                case RSide.Down:
                    return RSide.Up;
                case RSide.Up:
                    return RSide.Down;
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, "Wrong side passed");
            }
        }

        private RSide Step4GetCorrectRightSide(ref List<SolverFace[]> triplets)
        {
            //correct side should have triplet at correct position [2, 2]
            //     then is will be returned to treat it as right side in purpose of mapping
            //if we cant find at least one - we are performing algorithm on front side and trying again

            if (TripletAtCorrectPosition(triplets[0], (_lCenterColor, _fCenterColor), _dCenterColor))
                return RSide.Left;
            if (TripletAtCorrectPosition(triplets[1], (_fCenterColor, _rCenterColor), _dCenterColor))
                return RSide.Front;
            if (TripletAtCorrectPosition(triplets[2], (_rCenterColor, _bCenterColor), _dCenterColor))
                return RSide.Right;
            if (TripletAtCorrectPosition(triplets[3], (_bCenterColor, _lCenterColor), _dCenterColor))
                return RSide.Back;
            
            Step4PerformCornersSwapAlgorithm(RSide.Right, RSide.Left);
            triplets = GetCurrentLowerTriplets();

            if (TripletAtCorrectPosition(triplets[0], (_lCenterColor, _fCenterColor), _dCenterColor))
                return RSide.Left;
            if (TripletAtCorrectPosition(triplets[1], (_fCenterColor, _rCenterColor), _dCenterColor))
                return RSide.Front;
            if (TripletAtCorrectPosition(triplets[2], (_rCenterColor, _bCenterColor), _dCenterColor))
                return RSide.Right;
            if (TripletAtCorrectPosition(triplets[3], (_bCenterColor, _lCenterColor), _dCenterColor))
                return RSide.Back;
            
            Console.WriteLine("BUG");
            return RSide.Up;
        }

        private bool Step4TripletsAtCorrectCorners(IReadOnlyList<SolverFace[]> triplets)
        {
            if (!TripletAtCorrectPosition(triplets[0], (_lCenterColor, _fCenterColor), _dCenterColor))
                return false;
            if (!TripletAtCorrectPosition(triplets[1], (_fCenterColor, _rCenterColor), _dCenterColor))
                return false;
            if (!TripletAtCorrectPosition(triplets[2], (_rCenterColor, _bCenterColor), _dCenterColor))
                return false;
            if (!TripletAtCorrectPosition(triplets[3], (_bCenterColor, _lCenterColor), _dCenterColor))
                return false;

            return true;
        }
    }
}