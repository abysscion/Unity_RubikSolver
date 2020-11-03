using Util;

namespace Solver
{
    public class SolverSide
    {
        public readonly SolverFace[,] Faces;
        public SolverFace[] UNeighbours;
        public SolverFace[] DNeighbours;
        public SolverFace[] LNeighbours;
        public SolverFace[] RNeighbours;

        public SolverSide(RColor color)
        {
            Faces = new SolverFace[3, 3];
            for (var y = 0; y < 3; y++)
            {
                for (var x = 0; x < 3; x++)
                    Faces[x, y] = new SolverFace(color);
            }
        }
    }
}