using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Generator
{
    public class Generator
    {
        private Random random;
        public Generator ()
        {
            random = new Random(1337);
        }

        public Board Generate (int boardWidth, int boardHeight, int boardDepth, int numBombs)
        {
            Board board = new Board(boardWidth, boardHeight, boardDepth);

            for (int bombCount = 0; bombCount < numBombs; bombCount++)
            {
                PlaceBombOnBoard(board);
            }

            return board;
        }

        private void PlaceBombOnBoard(Board board)
        {
            
        }
    }
}
