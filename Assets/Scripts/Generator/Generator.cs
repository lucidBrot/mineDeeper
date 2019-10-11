using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Generator
{
    public class Generator
    {
        public Generator ()
        {

        }

        public Board generate (int boardWidth, int boardHeight, int boardDepth, int numBombs)
        {
            Board board = new Board(boardWidth, boardHeight, boardDepth);
            return board;
        }

    }
}
