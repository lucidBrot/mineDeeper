using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Assets.Scripts.Solver
{
    class Solver
    {
        private readonly Board board;
        private const int NUM_NEIGHBORS = 26;
        private bool? solvable;

        public Solver(Board board)
        {
            this.board = board;
        }

        public bool IsSolvable()
        {
            if (solvable == null)
            {
                Compute();
            }

            //TODO: return solvable.Value;
            return false;
        }

        private void Compute()
        {
            foreach (BoardCell cell in this.board.Cells)
            {
                ConsiderAllNeighborsAreBombs(cell);
                // TODO: consider no neighbors are bombs
            }
        }

        private void ConsiderAllNeighborsAreBombs(BoardCell cell)
        {
            // if the number of adjacent uncertainties equals the number on the cell, every uncertainty is a bomb
            int revealedSafeCells = 0;
            List<BoardCell> possibleBombs = new List<BoardCell>();
            foreach (BoardCell neighbor in board.GetAdjacentCells(cell.PosX, cell.PosY, cell.PosZ))
            {
                if (neighbor.IsRevealed && !neighbor.IsBomb)
                {
                    revealedSafeCells++;
                }

                if (!neighbor.IsRevealed)
                {
                    possibleBombs.Add(neighbor);
                }
            }

            if (revealedSafeCells + cell.AdjacentBombCount == NUM_NEIGHBORS)
            {
                // they are all bombs
                foreach (BoardCell bomb in possibleBombs)
                {
                    bomb.IsSuspect = true;
                }
            }
        }
    }
}
