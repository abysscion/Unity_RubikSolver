using Util;

namespace Solver
{
    public partial class RubikSolver
    {
        //for step3 points of interests are
        /*
                 [ ][ ][ ]
                 [ ][ ][ ]
                 [ ][ ][ ]        
        [ ][ ][ ][ ][ ][ ][ ][ ][ ][ ][ ][ ]
        [ ][ ][ ][ ][ ][ ][ ][ ][ ][ ][ ][ ]
        [ ][0][ ][ ][1][ ][ ][2][ ][ ][3][ ]
                 [ ][1][ ]
                 [0][ ][2]
                 [ ][3][ ]        
        */

        public void SolveStep3()
        {
            var lastRotationsCount = _rotationsMade.Count;

            if (!Step3CrossConditionsMet())
                Step3BuildCross();
            if (!Step3PairsConditionMet())
                Step3SwapPairs();
            
            RotationsCountStep3 = _rotationsMade.Count - lastRotationsCount;
        }

        private void Step3SwapPairs()
        {
            while (!Step3PairsConditionMet())
            {
                if (Step3TryToMatchPairsWithOneTurn())
                    break;
                if (!Step3TryToSwapReadyPairs())
                    PerformRotation(RSide.Down, RotationType.CounterClockwise);
            }
        }

        private bool Step3TryToSwapReadyPairs()
        {
            var rotationsCount = _rotationsMade.Count;
            
            if (_lFaces[1, 2].Color == _fCenterColor)
                Step3PerformPairsSwap(RSide.Back);
            else if (_fFaces[1, 2].Color == _rCenterColor)
                Step3PerformPairsSwap(RSide.Left);
            else if (_rFaces[1, 2].Color == _bCenterColor)
                Step3PerformPairsSwap(RSide.Front);
            else if (_bFaces[1, 2].Color == _lCenterColor)
                Step3PerformPairsSwap(RSide.Right);
            else if (_lFaces[1, 2].Color == _rCenterColor && _rFaces[1, 2].Color == _lCenterColor)
            {
                PerformRotation(RSide.Down, RotationType.Clockwise);
                Step3PerformPairsSwap(RSide.Left);
                Step3PerformPairsSwap(RSide.Right);
            }
            else if (_fFaces[1, 2].Color == _bCenterColor && _bFaces[1, 2].Color == _fCenterColor)
            {
                PerformRotation(RSide.Down, RotationType.Clockwise);
                Step3PerformPairsSwap(RSide.Front);
                Step3PerformPairsSwap(RSide.Back);
            }
            
            return rotationsCount != _rotationsMade.Count;
        }

        private void Step3PerformPairsSwap(RSide rMimic)
        {
            // R U R' U R U2 R' U
            // instead of up we are using down, because we won't turn rubik upside down programmatically
            const RSide uMimic = RSide.Down;
            
            PerformRotation(rMimic, RotationType.Clockwise);
            PerformRotation(uMimic, RotationType.Clockwise);
            PerformRotation(rMimic, RotationType.CounterClockwise);
            PerformRotation(uMimic, RotationType.Clockwise);
            PerformRotation(rMimic, RotationType.Clockwise);
            PerformRotation(uMimic, RotationType.Halfturn);
            PerformRotation(rMimic, RotationType.CounterClockwise);
            PerformRotation(uMimic, RotationType.Clockwise);
        }

        private bool Step3TryToMatchPairsWithOneTurn()
        {
            var rotationsCount = _rotationsMade.Count;
            
            if (_lFaces[1, 2].Color == _fCenterColor && _fFaces[1, 2].Color == _rCenterColor && 
                _rFaces[1, 2].Color == _bCenterColor && _bFaces[1, 2].Color == _lCenterColor)
                PerformRotation(RSide.Down, RotationType.Clockwise);
            else if (_lFaces[1, 2].Color == _bCenterColor && _bFaces[1, 2].Color == _rCenterColor && 
                     _rFaces[1, 2].Color == _fCenterColor && _fFaces[1, 2].Color == _lCenterColor)
                PerformRotation(RSide.Down, RotationType.CounterClockwise);
            else if (_lFaces[1, 2].Color == _rCenterColor && _fFaces[1, 2].Color == _bCenterColor && 
                     _rFaces[1, 2].Color == _lCenterColor && _bFaces[1, 2].Color == _fCenterColor)
                PerformRotation(RSide.Down, RotationType.Halfturn);
            
            return rotationsCount != _rotationsMade.Count;
        }
            
        private void Step3BuildCross()
        {
            //there could be only 4 possible configurations: empty, L, line and full cross
            const RSide rMimic = RSide.Left;
            const RSide uMimic = RSide.Down;
            var correctFacesCount = 0;
            var clrs = new[] {_dFaces[0, 1].Color, _dFaces[1, 0].Color, _dFaces[2, 1].Color, _dFaces[1, 2].Color};

            void CrossBuildingCommandsSequence()
            {
                PerformRotation(RSide.Front, RotationType.Clockwise);
                PerformRotation(rMimic, RotationType.Clockwise);
                PerformRotation(uMimic, RotationType.Clockwise);
                PerformRotation(rMimic, RotationType.CounterClockwise);
                PerformRotation(uMimic, RotationType.CounterClockwise);
                PerformRotation(RSide.Front, RotationType.CounterClockwise);
            }

            foreach (var color in clrs)
                correctFacesCount = color == _dCenterColor ? correctFacesCount + 1 : correctFacesCount;
            if (correctFacesCount == 0)
                CrossBuildingCommandsSequence();
            while (_dFaces[1, 0].Color == _dCenterColor || _dFaces[2, 1].Color != _dCenterColor)
                    PerformRotation(uMimic, RotationType.Clockwise);
            while (!Step3CrossConditionsMet())
                    CrossBuildingCommandsSequence();
        }

        private bool Step3CrossConditionsMet()
        {
            return _dFaces[0, 1].Color == _dCenterColor && _dFaces[1, 0].Color == _dCenterColor &&
                   _dFaces[2, 1].Color == _dCenterColor && _dFaces[1, 2].Color == _dCenterColor;
        }

        private bool Step3PairsConditionMet()
        {
            return _lFaces[1, 2].Color == _lCenterColor && _rFaces[1, 2].Color == _rCenterColor &&
                   _fFaces[1, 2].Color == _fCenterColor && _bFaces[1, 2].Color == _bCenterColor;
        }
    }
}