using System;
using System.Text.RegularExpressions;
using Util;

namespace Solver
{
    public class SolverCube
    {
        public readonly SolverSide[] Sides;
        
        private readonly Regex _commandSequenceFilter = new Regex(@"^[FBRLUD][2']?(?:(?:(\s[FBRLUD][2']?)+$)|$)");
        private readonly sbyte[] _uNBuf;
        private readonly sbyte[] _rNBuf;
        private readonly sbyte[] _lNBuf;
        private readonly sbyte[] _dNBuf;
        
        public SolverCube(SolverSide[] predefinedSides = null)
        {
            if (predefinedSides == null)
            {
                Sides = new SolverSide[6];
                for (var i = 0; i < Sides.Length; i++)
                    Sides[i] = new SolverSide((RColor) i);
            }
            else
                Sides = predefinedSides;

            _uNBuf = new sbyte[3];
            _rNBuf = new sbyte[3];
            _dNBuf = new sbyte[3];
            _lNBuf = new sbyte[3];
         
            SetUpNeighbours();
        }
        
        public void RotateSide(RSide targetSide, bool clockwise = true)   
        {
            var side = Sides[(sbyte) targetSide];

            RotateSelectedSideFaces(side, clockwise);
            RotateSelectedNeighbours(targetSide, side, clockwise);
        }

        public void RotateSide((RSide, RotationType) command)
        {
            var (targetSide, rotationType) = command;
            
            switch (rotationType)
            {
                case RotationType.Clockwise:
                    RotateSide(targetSide);
                    break;
                case RotationType.CounterClockwise:
                    RotateSide(targetSide, false);
                    break;
                case RotationType.Halfturn:
                    RotateSide(targetSide);
                    RotateSide(targetSide);
                    break;
            }
        }
        
        public void RotateByCommandSequence(string commandSequence)
        {
            if (!_commandSequenceFilter.IsMatch(commandSequence))
                throw new Exception("COMMAND SEQUENCE IS INVALID");
            foreach (var txt in commandSequence.Split(' '))
            {
                var (rotationType, targetSide) = Tools.TextToRotationCommand(txt);

                if (rotationType == RotationType.Halfturn)
                {
                    RotateSide(targetSide);
                    RotateSide(targetSide);
                }
                else
                    RotateSide(targetSide, rotationType == RotationType.Clockwise);
            }
        }
        
        private void SetUpNeighbours()
        {
            Sides[(sbyte) RSide.Front].UNeighbours = new[] {Sides[(sbyte) RSide.Up].Faces[0, 2], Sides[(sbyte) RSide.Up].Faces[1, 2], Sides[(sbyte) RSide.Up].Faces[2, 2]};
            Sides[(sbyte) RSide.Front].DNeighbours = new[] {Sides[(sbyte) RSide.Down].Faces[0, 0], Sides[(sbyte) RSide.Down].Faces[1, 0], Sides[(sbyte) RSide.Down].Faces[2, 0]};
            Sides[(sbyte) RSide.Front].LNeighbours = new[] {Sides[(sbyte) RSide.Left].Faces[2, 2], Sides[(sbyte) RSide.Left].Faces[2, 1], Sides[(sbyte) RSide.Left].Faces[2, 0]};
            Sides[(sbyte) RSide.Front].RNeighbours = new[] {Sides[(sbyte) RSide.Right].Faces[0, 0], Sides[(sbyte) RSide.Right].Faces[0, 1], Sides[(sbyte) RSide.Right].Faces[0, 2]};
         
            Sides[(sbyte) RSide.Left].UNeighbours = new[] {Sides[(sbyte) RSide.Up].Faces[0, 0], Sides[(sbyte) RSide.Up].Faces[0, 1], Sides[(sbyte) RSide.Up].Faces[0, 2]};
            Sides[(sbyte) RSide.Left].DNeighbours = new[] {Sides[(sbyte) RSide.Down].Faces[0, 0], Sides[(sbyte) RSide.Down].Faces[0, 1], Sides[(sbyte) RSide.Down].Faces[0, 2]};
            Sides[(sbyte) RSide.Left].LNeighbours = new[] {Sides[(sbyte) RSide.Back].Faces[2, 0], Sides[(sbyte) RSide.Back].Faces[2, 1], Sides[(sbyte) RSide.Back].Faces[2, 2]};
            Sides[(sbyte) RSide.Left].RNeighbours = new[] {Sides[(sbyte) RSide.Front].Faces[0, 0], Sides[(sbyte) RSide.Front].Faces[0, 1], Sides[(sbyte) RSide.Front].Faces[0, 2]};
         
            Sides[(sbyte) RSide.Right].UNeighbours = new[] {Sides[(sbyte) RSide.Up].Faces[2, 0], Sides[(sbyte) RSide.Up].Faces[2, 1], Sides[(sbyte) RSide.Up].Faces[2, 2]};
            Sides[(sbyte) RSide.Right].DNeighbours = new[] {Sides[(sbyte) RSide.Down].Faces[2, 0], Sides[(sbyte) RSide.Down].Faces[2, 1], Sides[(sbyte) RSide.Down].Faces[2, 2]};
            Sides[(sbyte) RSide.Right].LNeighbours = new[] {Sides[(sbyte) RSide.Front].Faces[2, 0], Sides[(sbyte) RSide.Front].Faces[2, 1], Sides[(sbyte) RSide.Front].Faces[2, 2]};
            Sides[(sbyte) RSide.Right].RNeighbours = new[] {Sides[(sbyte) RSide.Back].Faces[0, 0], Sides[(sbyte) RSide.Back].Faces[0, 1], Sides[(sbyte) RSide.Back].Faces[0, 2]};
         
            Sides[(sbyte) RSide.Back].UNeighbours = new[] {Sides[(sbyte) RSide.Up].Faces[0, 0], Sides[(sbyte) RSide.Up].Faces[1, 0], Sides[(sbyte) RSide.Up].Faces[2, 0]};
            Sides[(sbyte) RSide.Back].DNeighbours = new[] {Sides[(sbyte) RSide.Down].Faces[0, 2], Sides[(sbyte) RSide.Down].Faces[1, 2], Sides[(sbyte) RSide.Down].Faces[2, 2]};
            Sides[(sbyte) RSide.Back].LNeighbours = new[] {Sides[(sbyte) RSide.Right].Faces[2, 0], Sides[(sbyte) RSide.Right].Faces[2, 1], Sides[(sbyte) RSide.Right].Faces[2, 2]};
            Sides[(sbyte) RSide.Back].RNeighbours = new[] {Sides[(sbyte) RSide.Left].Faces[0, 0], Sides[(sbyte) RSide.Left].Faces[0, 1], Sides[(sbyte) RSide.Left].Faces[0, 2]};
         
            Sides[(sbyte) RSide.Up].UNeighbours = new[] {Sides[(sbyte) RSide.Back].Faces[0, 0], Sides[(sbyte) RSide.Back].Faces[1, 0], Sides[(sbyte) RSide.Back].Faces[2, 0]};
            Sides[(sbyte) RSide.Up].DNeighbours = new[] {Sides[(sbyte) RSide.Front].Faces[0, 0], Sides[(sbyte) RSide.Front].Faces[1, 0], Sides[(sbyte) RSide.Front].Faces[2, 0]};
            Sides[(sbyte) RSide.Up].LNeighbours = new[] {Sides[(sbyte) RSide.Left].Faces[0, 0], Sides[(sbyte) RSide.Left].Faces[1, 0], Sides[(sbyte) RSide.Left].Faces[2, 0]};
            Sides[(sbyte) RSide.Up].RNeighbours = new[] {Sides[(sbyte) RSide.Right].Faces[0, 0], Sides[(sbyte) RSide.Right].Faces[1, 0], Sides[(sbyte) RSide.Right].Faces[2, 0]};
         
            Sides[(sbyte) RSide.Down].UNeighbours = new[] {Sides[(sbyte) RSide.Front].Faces[0, 2], Sides[(sbyte) RSide.Front].Faces[1, 2], Sides[(sbyte) RSide.Front].Faces[2, 2]};
            Sides[(sbyte) RSide.Down].DNeighbours = new[] {Sides[(sbyte) RSide.Back].Faces[0, 2], Sides[(sbyte) RSide.Back].Faces[1, 2], Sides[(sbyte) RSide.Back].Faces[2, 2]};
            Sides[(sbyte) RSide.Down].LNeighbours = new[] {Sides[(sbyte) RSide.Left].Faces[0, 2], Sides[(sbyte) RSide.Left].Faces[1, 2], Sides[(sbyte) RSide.Left].Faces[2, 2]};
            Sides[(sbyte) RSide.Down].RNeighbours = new[] {Sides[(sbyte) RSide.Right].Faces[0, 2], Sides[(sbyte) RSide.Right].Faces[1, 2], Sides[(sbyte) RSide.Right].Faces[2, 2]};
        }
        
        private void RotateSelectedNeighbours(RSide targetSide, SolverSide solverSide, bool clockwise = true)
        {
            for (var i = 0; i < 3; i++)
            {
                _uNBuf[i] = solverSide.UNeighbours[i].Color;
                _rNBuf[i] = solverSide.RNeighbours[i].Color;
                _lNBuf[i] = solverSide.LNeighbours[i].Color;
                _dNBuf[i] = solverSide.DNeighbours[i].Color;
            }

            switch (targetSide)
            {
                case RSide.Front:
                    RotateNeighboursFront(solverSide, clockwise);
                    break;
                case RSide.Left:
                    RotateNeighboursLeft(solverSide, clockwise);
                    break;
                case RSide.Right:
                    RotateNeighboursRight(solverSide, clockwise);
                    break;
                case RSide.Down:
                    RotateNeighboursDown(solverSide, clockwise);
                    break;
                case RSide.Up:
                    RotateNeighboursUp(solverSide, clockwise);
                    break;
                case RSide.Back:
                    RotateNeighboursBack(solverSide, clockwise);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(targetSide), targetSide, null);
            }
        }

        private void RotateSelectedSideFaces(SolverSide solverSide, bool clockwise = true)
        {
            var faces = solverSide.Faces;
            sbyte tmp;
            sbyte tmp2;
            sbyte tmp3;

            if (clockwise)
            {
                tmp = faces[1, 0].Color;
                tmp2 = faces[2, 1].Color;
                tmp3 = faces[1, 2].Color;
                faces[1, 0].Color = faces[0, 1].Color;
                faces[2, 1].Color = tmp;
                faces[1, 2].Color = tmp2;
                faces[0, 1].Color = tmp3;
                tmp = faces[0, 0].Color;
                tmp2 = faces[2, 0].Color;
                tmp3 = faces[2, 2].Color;
                faces[0, 0].Color = faces[0, 2].Color;
                faces[2, 0].Color = tmp;
                faces[2, 2].Color = tmp2;
                faces[0, 2].Color = tmp3;
            }
            else
            {
                tmp = faces[0, 1].Color;
                tmp2 = faces[1, 2].Color;
                tmp3 = faces[2, 1].Color;
                faces[0, 1].Color = faces[1, 0].Color;
                faces[1, 2].Color = tmp;
                faces[2, 1].Color = tmp2;
                faces[1, 0].Color = tmp3;
                tmp = faces[0, 2].Color;
                tmp2 = faces[2, 2].Color;
                tmp3 = faces[2, 0].Color;
                faces[0, 2].Color = faces[0, 0].Color;
                faces[2, 2].Color = tmp;
                faces[2, 0].Color = tmp2;
                faces[0, 0].Color = tmp3;
            }
        }

        private void RotateNeighboursFront(SolverSide solverSide, bool clockwise = true)
        {
            if (clockwise)
            {
                for (var i = 0; i < 3; i++)
                {
                    solverSide.UNeighbours[i].Color = _lNBuf[i];
                    solverSide.RNeighbours[i].Color = _uNBuf[i];
                    solverSide.DNeighbours[i].Color = _rNBuf[2 - i];
                    solverSide.LNeighbours[i].Color = _dNBuf[2 - i];
                }
            }
            else
            {
                for (var i = 0; i < 3; i++)
                {
                    solverSide.UNeighbours[i].Color = _rNBuf[i];
                    solverSide.RNeighbours[i].Color = _dNBuf[2 - i];
                    solverSide.DNeighbours[i].Color = _lNBuf[2 - i];
                    solverSide.LNeighbours[i].Color = _uNBuf[i];
                }
            }
        }

        private void RotateNeighboursBack(SolverSide solverSide, bool clockwise = true)
        {
            if (clockwise)
            {
                for (var i = 0; i < 3; i++)
                {
                    solverSide.UNeighbours[i].Color = _lNBuf[i];
                    solverSide.RNeighbours[i].Color = _uNBuf[2 - i];
                    solverSide.DNeighbours[i].Color = _rNBuf[i];
                    solverSide.LNeighbours[i].Color = _dNBuf[2 - i];
                }
            }
            else
            {
                for (var i = 0; i < 3; i++)
                {
                    solverSide.UNeighbours[i].Color = _rNBuf[2 - i];
                    solverSide.RNeighbours[i].Color = _dNBuf[i];
                    solverSide.DNeighbours[i].Color = _lNBuf[2 - i];
                    solverSide.LNeighbours[i].Color = _uNBuf[i];
                }
            }
        }

        private void RotateNeighboursLeft(SolverSide solverSide, bool clockwise = true)
        {
            if (clockwise)
            {
                for (var i = 0; i < 3; i++)
                {
                    solverSide.UNeighbours[i].Color = _lNBuf[2 - i];
                    solverSide.RNeighbours[i].Color = _uNBuf[i];
                    solverSide.DNeighbours[i].Color = _rNBuf[i];
                    solverSide.LNeighbours[i].Color = _dNBuf[2 - i];
                }
            }
            else
            {
                for (var i = 0; i < 3; i++)
                {
                    solverSide.UNeighbours[i].Color = _rNBuf[i];
                    solverSide.RNeighbours[i].Color = _dNBuf[i];
                    solverSide.DNeighbours[i].Color = _lNBuf[2 - i];
                    solverSide.LNeighbours[i].Color = _uNBuf[2 - i];
                }
            }
        }

        private void RotateNeighboursRight(SolverSide solverSide, bool clockwise = true)
        {
            if (clockwise)
            {
                for (var i = 0; i < 3; i++)
                {
                    solverSide.UNeighbours[i].Color = _lNBuf[i];
                    solverSide.RNeighbours[i].Color = _uNBuf[2 - i];
                    solverSide.DNeighbours[i].Color = _rNBuf[2 - i];
                    solverSide.LNeighbours[i].Color = _dNBuf[i];
                }
            }
            else
            {
                for (var i = 0; i < 3; i++)
                {
                    solverSide.UNeighbours[i].Color = _rNBuf[2 - i];
                    solverSide.RNeighbours[i].Color = _dNBuf[2 - i];
                    solverSide.DNeighbours[i].Color = _lNBuf[i];
                    solverSide.LNeighbours[i].Color = _uNBuf[i];
                }
            }
        }

        private void RotateNeighboursUp(SolverSide solverSide, bool clockwise = true)
        {
            if (clockwise)
            {
                for (var i = 0; i < 3; i++)
                {
                    solverSide.UNeighbours[i].Color = _lNBuf[i];
                    solverSide.RNeighbours[i].Color = _uNBuf[i];
                    solverSide.DNeighbours[i].Color = _rNBuf[i];
                    solverSide.LNeighbours[i].Color = _dNBuf[i];
                }
            }
            else
            {
                for (var i = 0; i < 3; i++)
                {
                    solverSide.UNeighbours[i].Color = _rNBuf[i];
                    solverSide.RNeighbours[i].Color = _dNBuf[i];
                    solverSide.DNeighbours[i].Color = _lNBuf[i];
                    solverSide.LNeighbours[i].Color = _uNBuf[i];
                }
            }
        }

        private void RotateNeighboursDown(SolverSide solverSide, bool clockwise = true)
        {
            if (clockwise)
            {
                for (var i = 0; i < 3; i++)
                {
                    solverSide.UNeighbours[i].Color = _lNBuf[i];
                    solverSide.RNeighbours[i].Color = _uNBuf[i];
                    solverSide.DNeighbours[i].Color = _rNBuf[i];
                    solverSide.LNeighbours[i].Color = _dNBuf[i];
                }
            }
            else
            {
                for (var i = 0; i < 3; i++)
                {
                    solverSide.UNeighbours[i].Color = _rNBuf[i];
                    solverSide.RNeighbours[i].Color = _dNBuf[i];
                    solverSide.DNeighbours[i].Color = _lNBuf[i];
                    solverSide.LNeighbours[i].Color = _uNBuf[i];
                }
            }
        }
    }
}