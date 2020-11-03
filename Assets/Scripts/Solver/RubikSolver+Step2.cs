using Util;

namespace Solver
{
    public partial class RubikSolver
    {
        //for step2 points of interests are
        /*
                 [ ][ ][ ]
                 [ ][ ][ ]
                 [ ][ ][ ]        
        [ ][ ][ ][ ][ ][ ][ ][ ][ ][ ][ ][ ]
        [0][ ][0][1][ ][1][2][ ][2][3][ ][3]
        [ ][0][ ][ ][1][ ][ ][2][ ][ ][3][ ]
                 [ ][1][ ]
                 [0][ ][2]
                 [ ][3][ ]        
        */

        public void SolveStep2()
        {
            var lastRotationsCount = _rotationsMade.Count;
            
            while (!Step2ConditionsMet())
            {
                if (!Step2TryToSetReadyFace())
                {
                    if (Step2IsAnyFaceStuckAtWrongPosition())
                        Step2FixStuckFace();
                    else
                        PerformRotation(RSide.Down, RotationType.Clockwise);
                }
            }
            
            RotationsCountStep2 = _rotationsMade.Count - lastRotationsCount;
        }

        private void Step2PerformLeftAlgorithm(RSide frontMimic, RSide leftMimic)
        {
            // U' L' U L U F U' F'
            //instead of up we use down because we do not turn rubik upside down programmatically
            PerformRotation(RSide.Down, RotationType.CounterClockwise);
            PerformRotation(leftMimic, RotationType.CounterClockwise);
            PerformRotation(RSide.Down, RotationType.Clockwise);
            PerformRotation(leftMimic, RotationType.Clockwise);
            PerformRotation(RSide.Down, RotationType.Clockwise);
            PerformRotation(frontMimic, RotationType.Clockwise);
            PerformRotation(RSide.Down, RotationType.CounterClockwise);
            PerformRotation(frontMimic, RotationType.CounterClockwise);
        }
        
        private void Step2PerformRightAlgorithm(RSide frontMimic, RSide rightMimic)
        {
            // U R U' R' U' F' U F
            //instead of up we use down because we do not turn rubik upside down programmatically
            PerformRotation(RSide.Down, RotationType.Clockwise);
            PerformRotation(rightMimic, RotationType.Clockwise);
            PerformRotation(RSide.Down, RotationType.CounterClockwise);
            PerformRotation(rightMimic, RotationType.CounterClockwise);
            PerformRotation(RSide.Down, RotationType.CounterClockwise);
            PerformRotation(frontMimic, RotationType.CounterClockwise);
            PerformRotation(RSide.Down, RotationType.Clockwise);
            PerformRotation(frontMimic, RotationType.Clockwise);
        }
        
        private bool Step2TryToSetReadyFace()
        {
            //face is ready if it have 2 colors: first of side where it gonna be placed, second of neighbour side
            
            var rotationsCount = _rotationsMade.Count;

            if (_lFaces[1, 2].Color == _lCenterColor && _dFaces[0, 1].Color == _fCenterColor)
                Step2PerformLeftAlgorithm(RSide.Left, RSide.Front);
            else if (_lFaces[1, 2].Color == _lCenterColor && _dFaces[0, 1].Color == _bCenterColor)
                Step2PerformRightAlgorithm(RSide.Left, RSide.Back);
            else if (_fFaces[1, 2].Color == _fCenterColor && _dFaces[1, 0].Color == _rCenterColor)
                Step2PerformLeftAlgorithm(RSide.Front, RSide.Right);
            else if (_fFaces[1, 2].Color == _fCenterColor && _dFaces[1, 0].Color == _lCenterColor)
                Step2PerformRightAlgorithm(RSide.Front, RSide.Left);
            else if (_rFaces[1, 2].Color == _rCenterColor && _dFaces[2, 1].Color == _bCenterColor)
                Step2PerformLeftAlgorithm(RSide.Right, RSide.Back);
            else if (_rFaces[1, 2].Color == _rCenterColor && _dFaces[2, 1].Color == _fCenterColor)
                Step2PerformRightAlgorithm(RSide.Right, RSide.Front);
            else if (_bFaces[1, 2].Color == _bCenterColor && _dFaces[1, 2].Color == _lCenterColor)
                Step2PerformLeftAlgorithm(RSide.Back, RSide.Left);
            else if (_bFaces[1, 2].Color == _bCenterColor && _dFaces[1, 2].Color == _rCenterColor)
                Step2PerformRightAlgorithm(RSide.Back, RSide.Right);

            return rotationsCount != _rotationsMade.Count;
        }

        private void Step2FixStuckFace()
        {
            if (_bFaces[2, 1].Color != _bCenterColor || _lFaces[0, 1].Color != _lCenterColor)
                Step2PerformRightAlgorithm(RSide.Left, RSide.Back);
            else if (_lFaces[2, 1].Color != _lCenterColor || _fFaces[0, 1].Color != _fCenterColor)
                Step2PerformRightAlgorithm(RSide.Front, RSide.Left);
            else if (_fFaces[2, 1].Color != _fCenterColor || _rFaces[0, 1].Color != _rCenterColor)
                Step2PerformRightAlgorithm(RSide.Right, RSide.Front);
            else if (_rFaces[2, 1].Color != _rCenterColor || _bFaces[0, 1].Color != _bCenterColor)
                Step2PerformRightAlgorithm(RSide.Back, RSide.Right);
        }
        
        private bool Step2IsAnyFaceStuckAtWrongPosition()
        {
            //if algorithm cant find any ready face it starts to rotate down side for a case when face at wrong position
            //so if down side rotated 4 times in row without any new rotations it means that there's face at wrong pos.
            //actually it's the same as on Step1, where were down side rotations in similar case

            return HappenedFourUselessRotationsInARaw();
        }
        
        private bool Step2ConditionsMet()
        {
            return _lFaces[0, 1].Color == _lCenterColor && _lCenterColor == _lFaces[2, 1].Color &&
                   _fFaces[0, 1].Color == _fCenterColor && _fCenterColor == _fFaces[2, 1].Color &&
                   _rFaces[0, 1].Color == _rCenterColor && _rCenterColor == _rFaces[2, 1].Color &&
                   _bFaces[0, 1].Color == _bCenterColor && _bCenterColor == _bFaces[2, 1].Color;
        }
    }
}