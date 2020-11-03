using System;
using System.Collections.Generic;
using Util;

namespace Solver
{
    public partial class RubikSolver
    {
        private readonly List<(RSide, RotationType)> _rotationsMade;
        private readonly SolverFace[,] _lFaces;
        private readonly SolverFace[,] _rFaces;
        private readonly SolverFace[,] _uFaces;
        private readonly SolverFace[,] _dFaces;
        private readonly SolverFace[,] _fFaces;
        private readonly SolverFace[,] _bFaces;
        private readonly SolverCube _cube;
        private sbyte _lCenterColor;
        private sbyte _rCenterColor;
        private sbyte _uCenterColor;
        private sbyte _dCenterColor;
        private sbyte _fCenterColor;
        private sbyte _bCenterColor;

        public int TotalRotationsCount => _rotationsMade.Count;
        public int RotationsCountStep0 { get; private set; }
        public int RotationsCountStep1 { get; private set;}
        public int RotationsCountStep2 { get; private set;}
        public int RotationsCountStep3 { get; private set;}
        public int RotationsCountStep4 { get; private set; }

        public RubikSolver(SolverCube cube)
        {
            _cube = cube;
            _rotationsMade = new List<(RSide, RotationType)>();
            _lFaces = _cube.Sides[(sbyte) RSide.Left].Faces;
            _rFaces = _cube.Sides[(sbyte) RSide.Right].Faces;
            _uFaces = _cube.Sides[(sbyte) RSide.Up].Faces;
            _dFaces = _cube.Sides[(sbyte) RSide.Down].Faces;
            _fFaces = _cube.Sides[(sbyte) RSide.Front].Faces;
            _bFaces = _cube.Sides[(sbyte) RSide.Back].Faces;
            _lCenterColor = _lFaces[1, 1].Color;
            _rCenterColor = _rFaces[1, 1].Color;
            _uCenterColor = _uFaces[1, 1].Color;
            _dCenterColor = _dFaces[1, 1].Color;
            _fCenterColor = _fFaces[1, 1].Color;
            _bCenterColor = _bFaces[1, 1].Color;
        }
        
        public (RSide, RotationType)[] GetRotationsArray()
        {
            var rotations = new (RSide, RotationType)[_rotationsMade.Count];

            _rotationsMade.CopyTo(rotations);
            return rotations;
        }

        private void PerformRotation(RSide targetSide, RotationType rotationType)
        {
            switch (rotationType)
            {
                case RotationType.Clockwise:
                    _cube.RotateSide(targetSide);
                    break;
                case RotationType.CounterClockwise:
                    _cube.RotateSide(targetSide, false);
                    break;
                case RotationType.Halfturn:
                    _cube.RotateSide(targetSide);
                    _cube.RotateSide(targetSide);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rotationType), rotationType, null);
            }
            _rotationsMade.Add((targetSide, rotationType));
        }

        private bool HappenedFourUselessRotationsInARaw()
        {
            if (_rotationsMade.Count < 4)
                return false;

            var downSideTurnsCount = 0;

            for (var i = _rotationsMade.Count - 1; i >= _rotationsMade.Count - 4; i--)
            {
                if (_rotationsMade[i].Item1 == RSide.Down)
                    downSideTurnsCount++;
            }

            return downSideTurnsCount > 3;
        }

        private bool TripletAtCorrectPosition(SolverFace[] triplet, (sbyte, sbyte) matchColors, sbyte centerColor)
        {
            //triplet at right position when
            //    one of sides has same color as left(first) corner
            //    one of sides has same color as right(second) corner
            //    one of sides has same color as centerColor
                
            var (leftCorner, rightCorner) = matchColors;
            var passedConditions = 0;

            foreach (var face in triplet)
            {
                if (face.Color == leftCorner || face.Color == rightCorner || face.Color == centerColor)
                    passedConditions++;
            }

            return passedConditions == 3;
        }

        private List<SolverFace[]> GetCurrentLowerTriplets()
        {
            return new List<SolverFace[]>
            {
                new[] {_lFaces[2, 2], _fFaces[0, 2], _dFaces[0, 0]},
                new[] {_fFaces[2, 2], _rFaces[0, 2], _dFaces[2, 0]},
                new[] {_rFaces[2, 2], _bFaces[0, 2], _dFaces[2, 2]},
                new[] {_bFaces[2, 2], _lFaces[0, 2], _dFaces[0, 2]}
            };
        }
    }
}