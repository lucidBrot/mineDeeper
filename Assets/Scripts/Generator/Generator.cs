using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Generator
{
    public class Generator
    { 
        public Generator ()
        {
        }

        /// <summary>
        /// Generates a board with an arbitrary initial guess. Tries to solve that board, and if that works, returns it.
        /// </summary>
        /// <param name="boardWidth"></param>
        /// <param name="boardHeight"></param>
        /// <param name="boardDepth"></param>
        /// <param name="numBombs"></param>
        /// <returns></returns>
        public Board Generate (uint boardWidth, uint boardHeight, uint boardDepth, uint numBombs)
        {
            if (boardWidth * boardHeight * boardDepth < numBombs)
            {
                throw new ArgumentException("Number of bombs is larger than the Board");
            }

            Board board = new Board((int)boardWidth, (int)boardHeight, (int)boardDepth);

            for (uint bombCount = 0; bombCount < numBombs; bombCount++)
            {
                PlaceBombRandomlyOnBoard(board);
            }

            if (!(new Solver.Solver(board)).IsSolvable())
            {
                // retry randomly
                board = Generate(boardWidth, boardHeight, boardDepth, numBombs);
            }

            return board;
        }

        private void PlaceBombRandomlyOnBoard(Board board)
        {
            int posx = Random.Range(0, board.Width);
            int posy = Random.Range(0, board.Height);
            int posz = Random.Range(0, board.Depth);
            BoardCell cell = board.Cells[posx, posy, posz];
            Debug.Assert(cell.PosX == posx && cell.PosY==posy && cell.PosZ == posz, "Eric confused the order of axes");

            if (board.Cells[posx, posy, posz].IsBomb)
            {
                // try again with different random values
                PlaceBombRandomlyOnBoard(board);
            }
            else
            {
                board.SetBombState(posx, posy, posz, true);
            }
        }
    }
}
