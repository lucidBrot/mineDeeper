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
            int posx = Random.Range(0, board.Width);
            int posy = Random.Range(0, board.Height);
            int posz = Random.Range(0, board.Depth);
            BoardCell cell = board.Cells[posx, posy, posz];
            Debug.Assert(cell.PosX == posx && cell.PosY==posy && cell.PosZ == posz, "Eric confused the order of axes");

            board.SetBombState(posx, posy, posz, true);
        }
    }
}
