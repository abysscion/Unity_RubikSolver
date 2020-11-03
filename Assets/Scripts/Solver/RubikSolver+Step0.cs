using Util;

namespace Solver
{
    public partial class RubikSolver
    {
        public void SolveStep0()
        {
            var lastRotationsCount = _rotationsMade.Count;

            while (!Step0CrossConditionsMet())
            {
                while (Step0TryToBringCrossPartsWithOneTurn())
                    continue;
                if (Step0TryToResolveCrossPartDuplicate())
                    Step0TryToBringCrossPartsWithOneTurn();
                if (Step0TryToResolveUnreachablePart())
                    Step0TryToBringCrossPartsWithOneTurn();
            }

            while (!Step0PairsConditionMet())
            {
                if (!Step0TryToMatchPairsWithOneTurn())
                    if (!Step0TryToSwapReadyPairs())
                        PerformRotation(RSide.Up, RotationType.Clockwise);
            }

            RotationsCountStep0 = _rotationsMade.Count - lastRotationsCount;
        }

        private void Step0SwapNearPairsParts(RSide side0, RSide side1)
        {
            //swaps upper edges parts (if they are neighbours) without cross breaking

            if (side0 == RSide.Left && side1 == RSide.Front)
            {
                PerformRotation(RSide.Front, RotationType.CounterClockwise);
                PerformRotation(RSide.Up, RotationType.CounterClockwise);
                PerformRotation(RSide.Front, RotationType.Clockwise);
                PerformRotation(RSide.Up, RotationType.Clockwise);
                PerformRotation(RSide.Front, RotationType.CounterClockwise);
            }
            else if (side0 == RSide.Front && side1 == RSide.Right)
            {
                PerformRotation(RSide.Right, RotationType.CounterClockwise);
                PerformRotation(RSide.Up, RotationType.CounterClockwise);
                PerformRotation(RSide.Right, RotationType.Clockwise);
                PerformRotation(RSide.Up, RotationType.Clockwise);
                PerformRotation(RSide.Right, RotationType.CounterClockwise);
            }
            else if (side0 == RSide.Right && side1 == RSide.Back)
            {
                PerformRotation(RSide.Back, RotationType.CounterClockwise);
                PerformRotation(RSide.Up, RotationType.CounterClockwise);
                PerformRotation(RSide.Back, RotationType.Clockwise);
                PerformRotation(RSide.Up, RotationType.Clockwise);
                PerformRotation(RSide.Back, RotationType.CounterClockwise);
            }
            else if (side0 == RSide.Back && side1 == RSide.Left)
            {
                PerformRotation(RSide.Left, RotationType.CounterClockwise);
                PerformRotation(RSide.Up, RotationType.CounterClockwise);
                PerformRotation(RSide.Left, RotationType.Clockwise);
                PerformRotation(RSide.Up, RotationType.Clockwise);
                PerformRotation(RSide.Left, RotationType.CounterClockwise);
            }
        }

        private bool Step0TryToSwapReadyPairs()
        {
            //ready pairs are those which have two colors of each other
            
            if (_lFaces[1, 0].Color == _fCenterColor && _fFaces[1, 0].Color == _lCenterColor)
            {
                Step0SwapNearPairsParts(RSide.Left, RSide.Front);
                return true;
            }

            if (_fFaces[1, 0].Color == _rCenterColor && _rFaces[1, 0].Color == _fCenterColor)
            {
                Step0SwapNearPairsParts(RSide.Front, RSide.Right);
                return true;
            }

            if (_rFaces[1, 0].Color == _bCenterColor && _bFaces[1, 0].Color == _rCenterColor)
            {
                Step0SwapNearPairsParts(RSide.Right, RSide.Back);
                return true;
            }

            if (_bFaces[1, 0].Color == _lCenterColor && _lFaces[1, 0].Color == _bCenterColor)
            {
                Step0SwapNearPairsParts(RSide.Back, RSide.Left);
                return true;
            }

            return false;
        }

        private bool Step0TryToMatchPairsWithOneTurn()
        {
            //perform U, U' or U2 if it will lead to correct edges pairs
            
            if (_lFaces[1, 0].Color == _fCenterColor && _fFaces[1, 0].Color == _rCenterColor &&
                _rFaces[1, 0].Color == _bCenterColor && _bFaces[1, 0].Color == _lCenterColor)
            {
                PerformRotation(RSide.Up, RotationType.CounterClockwise);
                return true;
            }

            if (_lFaces[1, 0].Color == _bCenterColor && _bFaces[1, 0].Color == _rCenterColor &&
                _rFaces[1, 0].Color == _fCenterColor && _fFaces[1, 0].Color == _lCenterColor)
            {
                PerformRotation(RSide.Up, RotationType.Clockwise);
                return true;
            }

            if (_lFaces[1, 0].Color == _rCenterColor && _fFaces[1, 0].Color == _bCenterColor &&
                _rFaces[1, 0].Color == _lCenterColor && _bFaces[1, 0].Color == _fCenterColor)
            {
                PerformRotation(RSide.Up, RotationType.Halfturn);
                return true;
            }

            return false;
        }

        private bool Step0TryToResolveUnreachablePart()
        {
            //face is unreachable if it lies on upper layer or lower layer at middle, where
            //    one turn rotation cant move them onto the top
            //so we need to rotate side with such a face to make it possible to one turn top bringing
            
            //try to resolve it for left side
            if (_lFaces[1, 0].Color == _uCenterColor || _lFaces[1, 2].Color == _uCenterColor)
            {
                //if turning unreachable part side will break cross, turn cross to prevent it
                if (_uFaces[0, 1].Color == _uCenterColor)
                {
                    if (_uFaces[1, 0].Color != _uCenterColor)
                        PerformRotation(RSide.Up, RotationType.CounterClockwise);
                    else if (_uFaces[1, 2].Color != _uCenterColor)
                        PerformRotation(RSide.Up, RotationType.Clockwise);
                    else if (_uFaces[2, 1].Color != _uCenterColor)
                        PerformRotation(RSide.Up, RotationType.Halfturn);
                }

                PerformRotation(RSide.Left, RotationType.Clockwise);
                return true;
            }

            if (_fFaces[1, 0].Color == _uCenterColor || _fFaces[1, 2].Color == _uCenterColor)
            {
                if (_uFaces[1, 2].Color == _uCenterColor)
                {
                    if (_uFaces[2, 1].Color != _uCenterColor)
                        PerformRotation(RSide.Up, RotationType.Clockwise);
                    else if (_uFaces[0, 1].Color != _uCenterColor)
                        PerformRotation(RSide.Up, RotationType.CounterClockwise);
                    else if (_uFaces[1, 0].Color != _uCenterColor)
                        PerformRotation(RSide.Up, RotationType.Halfturn);
                }

                PerformRotation(RSide.Front, RotationType.Clockwise);
                return true;
            }

            if (_rFaces[1, 0].Color == _uCenterColor || _rFaces[1, 2].Color == _uCenterColor)
            {
                if (_uFaces[2, 1].Color == _uCenterColor)
                {
                    if (_uFaces[1, 0].Color != _uCenterColor)
                        PerformRotation(RSide.Up, RotationType.Clockwise);
                    else if (_uFaces[1, 2].Color != _uCenterColor)
                        PerformRotation(RSide.Up, RotationType.CounterClockwise);
                    else if (_uFaces[0, 1].Color != _uCenterColor)
                        PerformRotation(RSide.Up, RotationType.Halfturn);
                }

                PerformRotation(RSide.Right, RotationType.Clockwise);
                return true;
            }

            if (_bFaces[1, 0].Color == _uCenterColor || _bFaces[1, 2].Color == _uCenterColor)
            {
                if (_uFaces[1, 0].Color == _uCenterColor)
                {
                    if (_uFaces[0, 1].Color != _uCenterColor)
                        PerformRotation(RSide.Up, RotationType.Clockwise);
                    else if (_uFaces[2, 1].Color != _uCenterColor)
                        PerformRotation(RSide.Up, RotationType.CounterClockwise);
                    else if (_uFaces[1, 2].Color != _uCenterColor)
                        PerformRotation(RSide.Up, RotationType.Halfturn);
                }

                PerformRotation(RSide.Back, RotationType.Clockwise);
                return true;
            }

            return false;
        }

        private bool Step0TryToResolveCrossPartDuplicate()
        {
            //check for duplicates between right cross part and right side neighbours 
            if (_uFaces[2, 1].Color == _uCenterColor)
            {
                if (_fFaces[2, 1].Color == _uCenterColor || _dFaces[2, 1].Color == _uCenterColor ||
                    _bFaces[0, 1].Color == _uCenterColor)
                {
                    if (_uFaces[1, 0].Color != _uCenterColor)
                        PerformRotation(RSide.Up, RotationType.Clockwise);
                    else if (_uFaces[1, 2].Color != _uCenterColor)
                        PerformRotation(RSide.Up, RotationType.CounterClockwise);
                    else if (_uFaces[0, 1].Color != _uCenterColor)
                        PerformRotation(RSide.Up, RotationType.Halfturn);
                    return true;
                }
            }

            //check for duplicates between left cross part and left side neighbours 
            if (_uFaces[0, 1].Color == _uCenterColor)
            {
                if (_fFaces[0, 1].Color == _uCenterColor || _dFaces[0, 1].Color == _uCenterColor ||
                    _bFaces[2, 1].Color == _uCenterColor)
                {
                    if (_uFaces[1, 2].Color != _uCenterColor)
                        PerformRotation(RSide.Up, RotationType.Clockwise);
                    else if (_uFaces[1, 0].Color != _uCenterColor)
                        PerformRotation(RSide.Up, RotationType.CounterClockwise);
                    else if (_uFaces[2, 1].Color != _uCenterColor)
                        PerformRotation(RSide.Up, RotationType.Halfturn);
                    return true;
                }
            }

            //check for duplicates between up cross part and back side neighbours 
            if (_uFaces[1, 0].Color == _uCenterColor)
            {
                if (_lFaces[0, 1].Color == _uCenterColor || _dFaces[1, 2].Color == _uCenterColor ||
                    _rFaces[2, 1].Color == _uCenterColor)
                {
                    if (_uFaces[0, 1].Color != _uCenterColor)
                        PerformRotation(RSide.Up, RotationType.Clockwise);
                    else if (_uFaces[2, 1].Color != _uCenterColor)
                        PerformRotation(RSide.Up, RotationType.CounterClockwise);
                    else if (_uFaces[1, 2].Color != _uCenterColor)
                        PerformRotation(RSide.Up, RotationType.Halfturn);
                    return true;
                }
            }

            //check for duplicates between down cross part and front side neighbours 
            if (_uFaces[1, 2].Color == _uCenterColor)
            {
                if (_lFaces[2, 1].Color == _uCenterColor || _dFaces[1, 0].Color == _uCenterColor ||
                    _rFaces[0, 1].Color == _uCenterColor)
                {
                    if (_uFaces[2, 1].Color != _uCenterColor)
                        PerformRotation(RSide.Up, RotationType.Clockwise);
                    else if (_uFaces[0, 1].Color != _uCenterColor)
                        PerformRotation(RSide.Up, RotationType.CounterClockwise);
                    else if (_uFaces[1, 0].Color != _uCenterColor)
                        PerformRotation(RSide.Up, RotationType.Halfturn);
                    return true;
                }
            }

            return false;
        }

        private bool Step0TryToBringCrossPartsWithOneTurn()
        {
            var rotationsCountOnStart = _rotationsMade.Count;

            //upper cross part bringing from left/right/down side by rotating back side
            if (_uFaces[1, 0].Color != _uCenterColor)
            {
                if (_lFaces[0, 1].Color == _uCenterColor)
                    PerformRotation(RSide.Back, RotationType.CounterClockwise);
                else if (_rFaces[2, 1].Color == _uCenterColor)
                    PerformRotation(RSide.Back, RotationType.Clockwise);
                else if (_dFaces[1, 2].Color == _uCenterColor)
                    PerformRotation(RSide.Back, RotationType.Halfturn);
            }

            //left cross part bringing from front/back/down side by rotating left side
            if (_uFaces[0, 1].Color != _uCenterColor)
            {
                if (_fFaces[0, 1].Color == _uCenterColor)
                    PerformRotation(RSide.Left, RotationType.CounterClockwise);
                else if (_bFaces[2, 1].Color == _uCenterColor)
                    PerformRotation(RSide.Left, RotationType.Clockwise);
                else if (_dFaces[0, 1].Color == _uCenterColor)
                    PerformRotation(RSide.Left, RotationType.Halfturn);
            }

            //right cross part bringing from front/back/down side by rotating right side
            if (_uFaces[2, 1].Color != _uCenterColor)
            {
                if (_fFaces[2, 1].Color == _uCenterColor)
                    PerformRotation(RSide.Right, RotationType.Clockwise);
                else if (_bFaces[0, 1].Color == _uCenterColor)
                    PerformRotation(RSide.Right, RotationType.CounterClockwise);
                else if (_dFaces[2, 1].Color == _uCenterColor)
                    PerformRotation(RSide.Right, RotationType.Halfturn);
            }

            //down cross part bringing from left/right/down side by rotating front side
            if (_uFaces[1, 2].Color != _uCenterColor)
            {
                if (_lFaces[2, 1].Color == _uCenterColor)
                    PerformRotation(RSide.Front, RotationType.Clockwise);
                else if (_rFaces[0, 1].Color == _uCenterColor)
                    PerformRotation(RSide.Front, RotationType.CounterClockwise);
                else if (_dFaces[1, 0].Color == _uCenterColor)
                    PerformRotation(RSide.Front, RotationType.Halfturn);
            }

            return rotationsCountOnStart != _rotationsMade.Count;
        }

        private bool Step0CrossConditionsMet()
        {
            return _uFaces[0, 1].Color == _uCenterColor && _uFaces[1, 0].Color == _uCenterColor &&
                   _uFaces[2, 1].Color == _uCenterColor && _uFaces[1, 2].Color == _uCenterColor;
        }

        private bool Step0PairsConditionMet()
        {
            return _lFaces[1, 0].Color == _lCenterColor && _rFaces[1, 0].Color == _rCenterColor &&
                   _fFaces[1, 0].Color == _fCenterColor && _bFaces[1, 0].Color == _bCenterColor;
        }
    }
}